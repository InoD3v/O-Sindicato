import { render, screen, waitFor } from '@testing-library/react';
import { vi } from 'vitest';
import Home from '@/features/home/pages/Home/Home';
import * as healthService from '@/features/home/services/healthService';

// Mock the health service module
vi.mock('@/features/home/services/healthService', () => ({
  getHealth: vi.fn(),
}));

const mockGetHealth = vi.mocked(healthService.getHealth);

describe('Home', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('shows loading message initially', () => {
    mockGetHealth.mockReturnValue(new Promise(() => {})); // never resolves

    render(<Home />);

    expect(screen.getByText('Checking connection...')).toBeInTheDocument();
  });

  it('shows healthy status when API returns healthy', async () => {
    mockGetHealth.mockResolvedValue({
      status: 'healthy',
      database: true,
      timestamp: '2025-01-01T00:00:00Z',
    });

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByText('Connected')).toBeInTheDocument();
    });

    expect(screen.getByText('Database: Online')).toBeInTheDocument();
  });

  it('shows error badge when API call fails', async () => {
    mockGetHealth.mockRejectedValue(new Error('Network Error'));

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByText(/Disconnected: Network Error/)).toBeInTheDocument();
    });
  });

  it('shows database offline when database is false', async () => {
    mockGetHealth.mockResolvedValue({
      status: 'unhealthy',
      database: false,
      timestamp: '2025-01-01T00:00:00Z',
    });

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByText('Disconnected')).toBeInTheDocument();
    });

    expect(screen.getByText('Database: Offline')).toBeInTheDocument();
  });

  it('renders the title', () => {
    mockGetHealth.mockReturnValue(new Promise(() => {}));

    render(<Home />);

    expect(screen.getByText('O Sindicato')).toBeInTheDocument();
  });
});
