import { describe, it, expect, vi } from "vitest";
import { screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { renderWithProviders } from "../test-utils";
import SearchForm from "./SearchForm";

describe("SearchForm", () => {
  it("renders the input and button", () => {
    renderWithProviders(<SearchForm onSearch={vi.fn()} loading={false} />);

    expect(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Search" })).toBeInTheDocument();
  });

  it("calls onSearch with the trimmed query on submit", async () => {
    const onSearch = vi.fn();
    const user = userEvent.setup();

    renderWithProviders(<SearchForm onSearch={onSearch} loading={false} />);

    await user.type(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
      "  the hobbit  ",
    );
    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(onSearch).toHaveBeenCalledWith("the hobbit");
  });

  it("does not call onSearch for empty query", async () => {
    const onSearch = vi.fn();
    const user = userEvent.setup();

    renderWithProviders(<SearchForm onSearch={onSearch} loading={false} />);

    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(onSearch).not.toHaveBeenCalled();
  });

  it("does not call onSearch for whitespace-only query", async () => {
    const onSearch = vi.fn();
    const user = userEvent.setup();

    renderWithProviders(<SearchForm onSearch={onSearch} loading={false} />);

    await user.type(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
      "   ",
    );
    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(onSearch).not.toHaveBeenCalled();
  });

  it("disables input and shows loading spinner when loading", () => {
    renderWithProviders(<SearchForm onSearch={vi.fn()} loading={true} />);

    expect(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
    ).toBeDisabled();
    expect(screen.getByRole("button", { name: "Search" })).toBeDisabled();
  });

  it("submits on Enter key", async () => {
    const onSearch = vi.fn();
    const user = userEvent.setup();

    renderWithProviders(<SearchForm onSearch={onSearch} loading={false} />);

    const input = screen.getByPlaceholderText(
      "e.g. tolkien hobbit illustrated deluxe 1937",
    );
    await user.type(input, "dune");
    await user.keyboard("{Enter}");

    expect(onSearch).toHaveBeenCalledWith("dune");
  });
});
