import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { startSearch, streamResults } from "./books";
import type { BookCandidateResponse } from "./books";

beforeEach(() => {
  vi.spyOn(globalThis, "fetch").mockImplementation(() =>
    Promise.resolve(new Response()),
  );
});

afterEach(() => {
  vi.restoreAllMocks();
});

describe("startSearch", () => {
  it("sends a POST request with the encoded query", async () => {
    const mockJson = vi.fn().mockResolvedValue({ requestId: "abc-123" });
    vi.mocked(fetch).mockResolvedValue({
      ok: true,
      json: mockJson,
    } as unknown as Response);

    const result = await startSearch("tolkien hobbit");

    expect(fetch).toHaveBeenCalledWith(
      "/api/books/search?query=tolkien%20hobbit",
      { method: "POST" },
    );
    expect(result).toEqual({ requestId: "abc-123" });
  });

  it("throws when the response is not ok", async () => {
    vi.mocked(fetch).mockResolvedValue({ ok: false } as Response);

    await expect(startSearch("test")).rejects.toThrow("Failed to start search");
  });
});

describe("streamResults", () => {
  function mockStream(chunks: string[]) {
    const encoder = new TextEncoder();
    const stream = new ReadableStream({
      start(controller) {
        for (const chunk of chunks) {
          controller.enqueue(encoder.encode(chunk));
        }
        controller.close();
      },
    });
    vi.mocked(fetch).mockResolvedValue({
      ok: true,
      body: stream,
    } as unknown as Response);
  }

  it("calls onResponse for each SSE data line and onComplete when done", async () => {
    const event1: BookCandidateResponse = {
      matches: [
        {
          title: "The Hobbit",
          author: "Tolkien",
          first_publish_year: 1937,
          explanation: "A book",
          summary: "A book",
          link: "https://openlibrary.org",
          img_link: "https://openlibrary.org",
        },
      ],
      success: false,
    };
    const event2: BookCandidateResponse = {
      matches: [],
      success: true,
    };

    mockStream([
      `data: ${JSON.stringify(event1)}\n\ndata: ${JSON.stringify(event2)}\n\n`,
    ]);

    const onResponse = vi.fn();
    const onComplete = vi.fn();
    const onError = vi.fn();

    streamResults("req-1", onResponse, onComplete, onError);

    await vi.waitFor(() => {
      expect(onResponse).toHaveBeenCalledTimes(2);
      expect(onResponse).toHaveBeenNthCalledWith(1, event1);
      expect(onResponse).toHaveBeenNthCalledWith(2, event2);
      expect(onComplete).toHaveBeenCalledOnce();
      expect(onError).not.toHaveBeenCalled();
    });
  });

  it("calls onError on fetch failure", async () => {
    vi.mocked(fetch).mockRejectedValue(new Error("Network error"));

    const onResponse = vi.fn();
    const onComplete = vi.fn();
    const onError = vi.fn();

    streamResults("req-1", onResponse, onComplete, onError);

    await vi.waitFor(() => {
      expect(onError).toHaveBeenCalledWith(new Error("Network error"));
      expect(onComplete).not.toHaveBeenCalled();
    });
  });

  it("does not call onError on AbortError", async () => {
    const abortError = new Error("Aborted");
    abortError.name = "AbortError";
    vi.mocked(fetch).mockRejectedValue(abortError);

    const onResponse = vi.fn();
    const onComplete = vi.fn();
    const onError = vi.fn();

    streamResults("req-1", onResponse, onComplete, onError);

    await vi.waitFor(() => {
      expect(onError).not.toHaveBeenCalled();
    });
  });

  it("ignores malformed SSE data lines", async () => {
    mockStream([`data: not json\n\ndata: {"valid": true}\n\n`]);

    const onResponse = vi.fn();
    const onComplete = vi.fn();
    const onError = vi.fn();

    streamResults("req-1", onResponse, onComplete, onError);

    await vi.waitFor(() => {
      expect(onResponse).toHaveBeenCalledTimes(1);
      expect(onComplete).toHaveBeenCalledOnce();
      expect(onError).not.toHaveBeenCalled();
    });
  });

  it("handles partial chunks split across reads", async () => {
    const event: BookCandidateResponse = {
      matches: [
        {
          title: "A",
          author: "B",
          first_publish_year: 2000,
          explanation: "X",
          summary: "X",
          link: "https://openlibrary.org",
          img_link: "https://openlibrary.org",
        },
      ],
      success: true,
    };
    const json = JSON.stringify(event);

    mockStream([`data: ${json.slice(0, 10)}`, json.slice(10) + `\n\n`]);

    const onResponse = vi.fn();
    const onComplete = vi.fn();
    const onError = vi.fn();

    streamResults("req-1", onResponse, onComplete, onError);

    await vi.waitFor(() => {
      expect(onResponse).toHaveBeenCalledWith(event);
      expect(onComplete).toHaveBeenCalledOnce();
    });
  });

  it("throws when response is not ok", async () => {
    vi.mocked(fetch).mockResolvedValue({ ok: false } as Response);

    const onResponse = vi.fn();
    const onComplete = vi.fn();
    const onError = vi.fn();

    streamResults("req-1", onResponse, onComplete, onError);

    await vi.waitFor(() => {
      expect(onError).toHaveBeenCalledWith(new Error("Stream not found"));
    });
  });

  it("returns an AbortController that aborts the fetch", async () => {
    vi.mocked(fetch).mockImplementation(
      () => new Promise(() => {}), // never resolves
    );

    const controller = streamResults("req-1", vi.fn(), vi.fn(), vi.fn());

    controller.abort();
    expect(fetch).toHaveBeenCalledWith(
      "/api/books/stream/req-1",
      expect.objectContaining({
        signal: expect.objectContaining({ aborted: true }),
      }),
    );
  });
});
