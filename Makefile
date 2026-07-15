BACKEND_DIR := backend
CLIENT_DIR := client
DEPLOY_HOST := 10.0.0.11
APP_NAME := findthatbook
REMOTE_PATH := /opt/$(APP_NAME)
PINGAP_CONF_DIR := /opt/pingap/conf
SERVICE_NAME := $(APP_NAME)
SSH_TARGET := $(shell whoami)@$(DEPLOY_HOST)

-include .env
export

.PHONY: build build-backend build-client run run-backend run-client watch watch-backend watch-client lint lint-backend lint-client format format-check test test-client publish deploy deploy-pingap deploy-service

build: build-backend build-client

build-backend:
	dotnet build $(BACKEND_DIR)/Api/Api.csproj

build-client:
	npm --prefix $(CLIENT_DIR) run build

run:
	npm --prefix $(CLIENT_DIR) run start

watch:
	npm --prefix $(CLIENT_DIR) run watch

format: format-backend format-client

format-backend:
	dotnet format $(BACKEND_DIR)/Api/Api.csproj

format-client:
	npm --prefix $(CLIENT_DIR) run format

format-check: format-check-backend format-check-client

format-check-backend:
	dotnet format $(BACKEND_DIR)/Api/Api.csproj --verify-no-changes

format-check-client:
	npm --prefix $(CLIENT_DIR) run format:check

test: test-backend test-client

test-client:
	npm --prefix $(CLIENT_DIR) test

test-backend:
	dotnet test $(BACKEND_DIR)/Tests/Tests.csproj

scan:
	semgrep scan --config=auto

PUBLISH_DIR := deploy/publish

publish:
	@echo "--- Building client ---"
	npm --prefix $(CLIENT_DIR) run build
	@echo "--- Publishing backend for linux-x64 (self-contained) ---"
	dotnet publish $(BACKEND_DIR)/Api/Api.csproj \
		-c Release \
		-r linux-x64 \
		--self-contained \
		-o $(PUBLISH_DIR)
	cp -r $(CLIENT_DIR)/dist $(PUBLISH_DIR)/wwwroot

deploy: deploy-pingap deploy-service
	@echo "--- Deployment complete ---"

deploy-service: publish
	@echo "--- Ensuring remote path and user ---"
	ssh -t $(SSH_TARGET) "sudo install -d -o $(SERVICE_NAME) -g $(SERVICE_NAME) $(REMOTE_PATH) 2>/dev/null; if ! id $(SERVICE_NAME) &>/dev/null; then sudo useradd --system --home $(REMOTE_PATH) --shell /usr/sbin/nologin $(SERVICE_NAME); sudo install -d -o $(SERVICE_NAME) -g $(SERVICE_NAME) $(REMOTE_PATH); fi"	
	@echo "--- Syncing published app ---"
	rsync -a --delete $(CLIENT_DIR)/dist/ $(PUBLISH_DIR)/wwwroot/
	rsync -az --delete --chown=$(SERVICE_NAME):$(SERVICE_NAME) --rsync-path="sudo rsync" $(PUBLISH_DIR)/ $(SSH_TARGET):$(REMOTE_PATH)/
	@echo "--- Deploying .env.production ---"
	scp .env.production $(SSH_TARGET):/tmp/$(APP_NAME).env
	ssh $(SSH_TARGET) "sudo mv /tmp/$(APP_NAME).env $(REMOTE_PATH)/.env && sudo chown $(SERVICE_NAME):$(SERVICE_NAME) $(REMOTE_PATH)/.env && sudo chmod 600 $(REMOTE_PATH)/.env"
	@echo "--- Installing systemd unit ---"
	rsync -az --rsync-path="sudo rsync" deploy/$(SERVICE_NAME).service $(SSH_TARGET):/etc/systemd/system/
	ssh -t $(SSH_TARGET) "sudo systemctl daemon-reload && sudo systemctl enable $(SERVICE_NAME)"
	@echo "--- Restarting service ---"
	ssh -t $(SSH_TARGET) "sudo systemctl restart $(SERVICE_NAME) && sudo systemctl --no-pager status $(SERVICE_NAME) | head -10"

deploy-pingap:
	@echo "--- Copying findthatbook pingap config ---"
	scp deploy/pingap/findthatbook.toml $(SSH_TARGET):$(PINGAP_CONF_DIR)/
	@echo "--- Adding findthatbook to https server locations ---"
	ssh $(SSH_TARGET) "sed -i 's/locations = \\[.*\\]/locations = [\"blog\", \"gitlab\", \"iis\", \"jellyfin\", \"santiagodevelopment\", \"findthatbook\"]/' $(PINGAP_CONF_DIR)/servers.https.toml"
	@echo "--- Pingap autoreloads config automatically (--autoreload) ---"