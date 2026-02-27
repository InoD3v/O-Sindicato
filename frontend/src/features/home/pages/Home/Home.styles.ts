import styled from 'styled-components';

export const Container = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  font-family: system-ui, -apple-system, sans-serif;
  background-color: #0f0f0f;
  color: #ffffff;
`;

export const Title = styled.h1`
  font-size: 2.5rem;
  margin-bottom: 1rem;
`;

export const StatusBadge = styled.span<{ $healthy: boolean }>`
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 9999px;
  font-weight: 600;
  font-size: 0.875rem;
  background-color: ${({ $healthy }) => ($healthy ? '#16a34a' : '#dc2626')};
  color: #ffffff;
`;

export const Info = styled.p`
  color: #a1a1aa;
  margin-top: 0.5rem;
  font-size: 0.875rem;
`;
