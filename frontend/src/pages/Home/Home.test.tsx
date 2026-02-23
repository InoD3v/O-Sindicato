import { render, screen, waitFor } from '@testing-library/react';
import { vi, type Mock } from 'vitest';
import Home from '@/pages/Home/Home';
import api from '@/services/api';

// Mock the API module
vi.mock('@/services/api', () => ({
  default: {
    get: vi.fn(),
    interceptors: {
      request: { use: vi.fn() },
      response: { use: vi.fn() },
    },
  },
}));

describe('Home', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('shows loading message initially', () => {
    (api.get as Mock).mockReturnValue(new Promise(() => {})); // never resolves

    render(<Home />);

    expect(screen.getByText('Checking connection...')).toBeInTheDocument();
  });

  it('shows healthy status when API returns healthy', async () => {
    (api.get as Mock).mockResolvedValue({
      data: {
        status: 'healthy',
        database: true,
        timestamp: '2025-01-01T00:00:00Z',
      },
    });

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByText('Connected')).toBeInTheDocument();
    });

    expect(screen.getByText('Database: Online')).toBeInTheDocument();
  });

  it('shows error badge when API call fails', async () => {
    (api.get as Mock).mockRejectedValue(new Error('Network Error'));

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByText(/Disconnected: Network Error/)).toBeInTheDocument();
    });
  });

  it('shows database offline when database is false', async () => {
    (api.get as Mock).mockResolvedValue({
      data: {
        status: 'unhealthy',
        database: false,
        timestamp: '2025-01-01T00:00:00Z',
      },
    });

    render(<Home />);

    await waitFor(() => {
      expect(screen.getByText('Disconnected')).toBeInTheDocument();
    });

    expect(screen.getByText('Database: Offline')).toBeInTheDocument();
  });

  it('renders the title', () => {
    (api.get as Mock).mockReturnValue(new Promise(() => {}));

    render(<Home />);

    expect(screen.getByText('O Sindicato')).toBeInTheDocument();
  });
});
