import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import App from '@/App';

function renderApp(route = '/') {
  return render(
    <MemoryRouter initialEntries={[route]}>
      <App />
    </MemoryRouter>
  );
}

describe('App', () => {
  it('renders the home page on /', () => {
    renderApp('/');
    expect(screen.getByText('O Sindicato')).toBeInTheDocument();
  });
});
