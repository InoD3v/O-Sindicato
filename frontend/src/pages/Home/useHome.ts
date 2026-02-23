import { useEffect, useState } from 'react';
import api from '../../services/api';

interface HealthStatus {
  status: string;
  database: boolean;
  timestamp: string;
}

export function useHome() {
  const [health, setHealth] = useState<HealthStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api
      .get<HealthStatus>('/api/health')
      .then((res) => setHealth(res.data))
      .catch((err: Error) => setError(err.message));
  }, []);

  const isLoading = !health && !error;

  return { health, error, isLoading };
}
