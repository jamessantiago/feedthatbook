import "@mantine/core/styles.css";
import { useState, useRef } from "react";
import { AppShell, Container, Group, Title } from "@mantine/core";
import { MantineProvider } from "@mantine/core";
import { theme } from "./theme";
import SearchForm from "./components/SearchForm";
import ResultsList from "./components/ResultsList";
import { startSearch, streamResults, type BookCandidate } from "./api/books";

export default function App() {
  const [candidates, setCandidates] = useState<BookCandidate[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [done, setDone] = useState(false);
  const abortRef = useRef<AbortController | null>(null);

  const handleSearch = async (query: string) => {
    abortRef.current?.abort();

    setLoading(true);
    setError(null);
    setCandidates([]);
    setDone(false);

    try {
      const { requestId } = await startSearch(query);

      abortRef.current = streamResults(
        requestId,
        (response) => {
          if (!response.success && response.error) {
            setError(response.error);
            setLoading(false);
            return;
          }
          setCandidates(response.matches ?? []);
          setDone(response.success);
        },
        () => {
          setDone(true);
          setLoading(false);
        },
        (err) => {
          setError(err.message);
          setLoading(false);
        },
      );
    } catch (err) {
      setError(err instanceof Error ? err.message : "Search failed");
      setLoading(false);
    }
  };

  return (
    <MantineProvider theme={theme}>
      <AppShell header={{ height: 60 }} padding="md">
        <AppShell.Header>
          <Container h="100%">
            <Group h="100%" align="center">
              <Title order={3}>Find That Book</Title>
            </Group>
          </Container>
        </AppShell.Header>

        <AppShell.Main>
          <Container size="sm" py="xl">
            <SearchForm onSearch={handleSearch} loading={loading} />
            <ResultsList candidates={candidates} error={error} done={done} />
          </Container>
        </AppShell.Main>
      </AppShell>
    </MantineProvider>
  );
}
