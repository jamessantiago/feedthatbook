import type { ReactNode } from "react";
import { MantineProvider } from "@mantine/core";
import { render } from "@testing-library/react";

function TestWrapper({ children }: { children: ReactNode }) {
  return <MantineProvider>{children}</MantineProvider>;
}

function renderWithProviders(ui: ReactNode) {
  return render(ui, { wrapper: TestWrapper });
}

export { renderWithProviders };
