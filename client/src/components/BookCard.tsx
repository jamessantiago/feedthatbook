import { Card, Group, Text, Title } from "@mantine/core";
import type { BookCandidate } from "../api/books";

interface BookCardProps {
  candidate: BookCandidate;
}

export default function BookCard({ candidate }: BookCardProps) {
  return (
    <Card shadow="sm" padding="md" radius="md" withBorder>
      <Group justify="space-between" mb="xs">
        <Title order={4}>{candidate.title}</Title>
        {candidate.first_publish_year > 0 && (
          <Text c="dimmed" size="sm">
            {candidate.first_publish_year}
          </Text>
        )}
      </Group>

      <Text size="sm" c="dimmed" mb="sm">
        {candidate.author}
      </Text>

      <Text size="sm">{candidate.explanation}</Text>
    </Card>
  );
}
