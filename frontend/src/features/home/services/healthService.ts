import type { HealthStatus } from '@/features/home/types/health.types';
import api from '@/services/api';

export async function getHealth(): Promise<HealthStatus> {
  const { data } = await api.get<HealthStatus>('/api/health');
  return data;
}
