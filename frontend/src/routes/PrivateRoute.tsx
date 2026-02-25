import { Navigate } from 'react-router-dom';

interface PrivateRouteProps {
  children: React.ReactNode;
}

/**
 * Wraps a route that requires authentication.
 * TODO: Replace `isAuthenticated` with real auth check (e.g. useAuth() hook).
 */
export function PrivateRoute({ children }: PrivateRouteProps) {
  const isAuthenticated = Boolean(localStorage.getItem('token'));

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return <>{children}</>;
}
