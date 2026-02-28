import { useEffect, useState } from 'react';
import type { HealthStatus } from '@/features/home/types/health.types';
import { getHealth } from '@/features/home/services/healthService';

export function useHome() {
  const [health, setHealth] = useState<HealthStatus | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    getHealth()
      .then(setHealth)
      .catch((err: Error) => setError(err.message));
  }, []);

  const isLoading = !health && !error;

  return { health, error, isLoading };
}
