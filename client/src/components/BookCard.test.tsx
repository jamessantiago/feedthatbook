import { describe, it, expect } from "vitest";
import { screen } from "@testing-library/react";
import { renderWithProviders } from "../test-utils";
import BookCard from "./BookCard";
import type { BookCandidate } from "../api/books";

const baseCandidate: BookCandidate = {
  title: "The Hobbit",
  author: "J.R.R. Tolkien",
  first_publish_year: 1937,
  explanation: "A fantasy novel about a hobbit's adventure.",
};

describe("BookCard", () => {
  it("renders title, author, year, and explanation", () => {
    renderWithProviders(<BookCard candidate={baseCandidate} />);

    expect(screen.getByText("The Hobbit")).toBeInTheDocument();
    expect(screen.getByText("J.R.R. Tolkien")).toBeInTheDocument();
    expect(screen.getByText("1937")).toBeInTheDocument();
    expect(
      screen.getByText("A fantasy novel about a hobbit's adventure."),
    ).toBeInTheDocument();
  });

  it("does not render year when first_publish_year is 0", () => {
    renderWithProviders(
      <BookCard candidate={{ ...baseCandidate, first_publish_year: 0 }} />,
    );

    expect(screen.getByText("The Hobbit")).toBeInTheDocument();
    expect(screen.queryByText("0")).not.toBeInTheDocument();
  });

  it("does not render year when first_publish_year is negative", () => {
    renderWithProviders(
      <BookCard candidate={{ ...baseCandidate, first_publish_year: -1 }} />,
    );

    expect(screen.getByText("The Hobbit")).toBeInTheDocument();
    expect(screen.queryByText("-1")).not.toBeInTheDocument();
  });
});
