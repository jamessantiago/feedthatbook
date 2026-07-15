import { Card, Group, Text, Title, Image, Spoiler } from "@mantine/core";
import type { BookCandidate } from "../api/books";

interface BookCardProps {
  candidate: BookCandidate;
}

export default function BookCard({ candidate }: BookCardProps) {
  return (
    <Card
      shadow="sm"
      padding="md"
      radius="md"
      component="a"
      withBorder
      href={candidate.link}
      target="_blank"
      orientation="horizontal"
    >
      <Card.Section>
        <Image
          src={candidate.img_link}
          h={180}
          w={110}
          fit="cover"
          radius="sm"
          mr={10}
        />
      </Card.Section>

      <div style={{ flex: 1 }}>
        <Group justify="space-between" align="flex-start" wrap="nowrap" mb="xs">
          <Title order={4} flex={1} lineClamp={2}>
            {candidate.title}
          </Title>

          {candidate.first_publish_year > 0 && (
            <Text c="dimmed" size="sm" ml="sm">
              {candidate.first_publish_year}
            </Text>
          )}
        </Group>

        <Text size="sm" c="dimmed" mb="sm">
          {candidate.author}
        </Text>

        <Text size="sm">{candidate.summary}</Text>

        <Text size="xs" c="dimmed" mt="sm" fs="italic">
          Why this match: {candidate.explanation}
        </Text>
      </div>
    </Card>
  );
}
