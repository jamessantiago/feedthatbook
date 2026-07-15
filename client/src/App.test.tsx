import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import App from "./App";

function mockStream(body: string) {
  const encoder = new TextEncoder();
  const stream = new ReadableStream({
    start(controller) {
      controller.enqueue(encoder.encode(body));
      controller.close();
    },
  });
  return stream;
}

beforeEach(() => {
  vi.spyOn(globalThis, "fetch").mockImplementation(() =>
    Promise.resolve(new Response()),
  );
});

afterEach(() => {
  vi.restoreAllMocks();
});

describe("App", () => {
  it("renders the header and search form", () => {
    render(<App />);

    expect(screen.getByText("Find That Book")).toBeInTheDocument();
    expect(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Search" })).toBeInTheDocument();
  });

  it("performs a search and displays results", async () => {
    const user = userEvent.setup();

    vi.mocked(fetch)
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ requestId: "req-1" }),
      } as Response)
      .mockResolvedValueOnce({
        ok: true,
        body: mockStream(
          `data: {"matches":[{"title":"The Hobbit","author":"Tolkien","first_publish_year":1937,"explanation":"A book."}],"success":true}\n\n`,
        ),
      } as Response);

    render(<App />);

    await user.type(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
      "hobbit",
    );
    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(await screen.findByText("The Hobbit")).toBeInTheDocument();
  });

  it("shows error alert when the stream returns an error", async () => {
    const user = userEvent.setup();

    vi.mocked(fetch)
      .mockResolvedValueOnce({
        ok: true,
        json: async () => ({ requestId: "req-1" }),
      } as Response)
      .mockResolvedValueOnce({
        ok: true,
        body: mockStream(
          `data: {"matches":[],"success":false,"error":"Rate limit exceeded"}\n\n`,
        ),
      } as Response);

    render(<App />);

    await user.type(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
      "hobbit",
    );
    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(
      await screen.findByText(
        "We're unable to search for books right now. Please try again later.",
      ),
    ).toBeInTheDocument();
  });

  it("shows error alert when startSearch fails", async () => {
    const user = userEvent.setup();

    vi.mocked(fetch).mockRejectedValue(new Error("Network error"));

    render(<App />);

    await user.type(
      screen.getByPlaceholderText(
        "e.g. tolkien hobbit illustrated deluxe 1937",
      ),
      "hobbit",
    );
    await user.click(screen.getByRole("button", { name: "Search" }));

    expect(
      await screen.findByText(
        "We're unable to search for books right now. Please try again later.",
      ),
    ).toBeInTheDocument();
  });
});
