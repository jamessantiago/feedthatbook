import { useState } from "react";
import { Button, TextInput } from "@mantine/core";

interface SearchFormProps {
  onSearch: (query: string) => void;
  loading: boolean;
}

export default function SearchForm({ onSearch, loading }: SearchFormProps) {
  const [query, setQuery] = useState("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (query.trim()) onSearch(query.trim());
  };

  return (
    <form
      onSubmit={handleSubmit}
      style={{ marginBottom: "var(--mantine-spacing-xl)" }}
    >
      <TextInput
        placeholder="e.g. tolkien hobbit illustrated deluxe 1937"
        value={query}
        onChange={(e) => setQuery(e.currentTarget.value)}
        size="lg"
        disabled={loading}
        rightSection={
          <Button type="submit" loading={loading}>
            Search
          </Button>
        }
        rightSectionWidth={90}
      />
    </form>
  );
}
