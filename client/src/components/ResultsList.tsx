import { Alert, Stack, Text } from "@mantine/core";
import { IconInfoCircle } from "@tabler/icons-react";
import BookCard from "./BookCard";
import type { BookCandidate } from "../api/books";

interface ResultsListProps {
  candidates: BookCandidate[];
  error: string | null;
  done: boolean;
}

export default function ResultsList({
  candidates,
  error,
  done,
}: ResultsListProps) {
  if (error) {
    console.log(error);
    return (
      <Alert color="red" title="Error">
        We're unable to search for books right now. Please try again later.
      </Alert>
    );
  }

  if (!done && candidates.length === 0) return null;

  if (done && candidates.length === 0) {
    return (
      <Alert icon={<IconInfoCircle />} color="blue" title="No results">
        <Text>No books found for your query. Try a different search term.</Text>
      </Alert>
    );
  }

  return (
    <Stack gap="md">
      {candidates.map((c, i) => (
        <BookCard key={`${c.title}-${c.author}-${i}`} candidate={c} />
      ))}
    </Stack>
  );
}
