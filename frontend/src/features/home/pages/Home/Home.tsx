import { useHome } from './useHome';
import { Container, Title, StatusBadge, Info } from './Home.styles';

function Home() {
  const { health, error, isLoading } = useHome();

  return (
    <Container>
      <Title>O Sindicato</Title>
      {error && <StatusBadge $healthy={false}>Disconnected: {error}</StatusBadge>}
      {health && (
        <>
          <StatusBadge $healthy={health.database}>
            {health.status === 'healthy' ? 'Connected' : 'Disconnected'}
          </StatusBadge>
          <Info>Database: {health.database ? 'Online' : 'Offline'}</Info>
          <Info>Timestamp: {health.timestamp}</Info>
        </>
      )}
      {isLoading && <Info>Checking connection...</Info>}
    </Container>
  );
}

export default Home;
