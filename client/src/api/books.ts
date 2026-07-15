export interface BookCandidate {
  title: string;
  author: string;
  first_publish_year: number;
  explanation: string;
  summary: string;
  link: string;
  img_link: string;
}

export interface BookCandidateResponse {
  matches: BookCandidate[];
  success: boolean;
  error?: string | null;
}

export interface StartSearchResult {
  requestId: string;
}

export async function startSearch(query: string): Promise<StartSearchResult> {
  const res = await fetch(
    `/api/books/search?query=${encodeURIComponent(query)}`,
    { method: "POST" },
  );
  if (!res.ok) throw new Error("Failed to start search");
  return res.json();
}

export function streamResults(
  requestId: string,
  onResponse: (response: BookCandidateResponse) => void,
  onComplete: () => void,
  onError: (error: Error) => void,
): AbortController {
  const controller = new AbortController();

  fetch(`/api/books/stream/${requestId}`, { signal: controller.signal })
    .then(async (res) => {
      if (!res.ok) throw new Error("Stream not found");
      const reader = res.body!.getReader();
      const decoder = new TextDecoder();
      let buffer = "";

      while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        buffer += decoder.decode(value, { stream: true });
        const lines = buffer.split("\n");
        buffer = lines.pop() ?? "";

        for (const line of lines) {
          if (line.startsWith("data: ")) {
            try {
              const data: BookCandidateResponse = JSON.parse(line.slice(6));
              onResponse(data);
            } catch {
              /* ignore malformed SSE data */
            }
          }
        }
      }

      onComplete();
    })
    .catch((err) => {
      if (err.name !== "AbortError") onError(err);
    });

  return controller;
}
