import { describe, it, expect } from "vitest";
import { screen } from "@testing-library/react";
import { renderWithProviders } from "../test-utils";
import ResultsList from "./ResultsList";
import type { BookCandidate } from "../api/books";

const candidates: BookCandidate[] = [
  {
    title: "The Hobbit",
    author: "Tolkien",
    first_publish_year: 1937,
    explanation: "A fantasy novel.",
  },
  {
    title: "Dune",
    author: "Herbert",
    first_publish_year: 1965,
    explanation: "A sci-fi novel.",
  },
];

describe("ResultsList", () => {
  it("renders error alert when error is set", () => {
    renderWithProviders(
      <ResultsList candidates={[]} error="Something went wrong" done={false} />,
    );

    expect(screen.getByText("Error")).toBeInTheDocument();
    expect(screen.getByText("Something went wrong")).toBeInTheDocument();
  });

  it("renders nothing while loading with no candidates", () => {
    renderWithProviders(
      <ResultsList candidates={[]} error={null} done={false} />,
    );

    expect(screen.queryByRole("alert")).not.toBeInTheDocument();
    expect(screen.queryByText("No results")).not.toBeInTheDocument();
  });

  it("renders empty alert when done with no candidates", () => {
    renderWithProviders(
      <ResultsList candidates={[]} error={null} done={true} />,
    );

    expect(screen.getByText("No results")).toBeInTheDocument();
    expect(
      screen.getByText(
        "No books found for your query. Try a different search term.",
      ),
    ).toBeInTheDocument();
  });

  it("renders book cards when candidates exist", () => {
    renderWithProviders(
      <ResultsList candidates={candidates} error={null} done={false} />,
    );

    expect(screen.getByText("The Hobbit")).toBeInTheDocument();
    expect(screen.getByText("Dune")).toBeInTheDocument();
  });

  it("renders book cards when done and candidates exist", () => {
    renderWithProviders(
      <ResultsList candidates={candidates} error={null} done={true} />,
    );

    expect(screen.getByText("The Hobbit")).toBeInTheDocument();
    expect(screen.getByText("Dune")).toBeInTheDocument();
  });

  it("prioritizes error over other states", () => {
    renderWithProviders(
      <ResultsList candidates={candidates} error="Server error" done={false} />,
    );

    expect(screen.getByText("Error")).toBeInTheDocument();
    expect(screen.queryByText("The Hobbit")).not.toBeInTheDocument();
  });
});
