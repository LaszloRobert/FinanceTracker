import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Toaster } from '@/components/ui/sonner';
import { AuthProvider } from '@/features/auth/auth-provider';
import { ProtectedRoute } from '@/features/auth/components/protected-route';
import { AppLayout } from '@/components/layout/app-layout';
import { LoginPage } from '@/features/auth/pages/login';
import { RegisterPage } from '@/features/auth/pages/register';
import { DashboardPage } from '@/features/dashboard/pages/dashboard';
import { TransactionsPage } from '@/features/transactions/pages/transactions';
import { CategoriesPage } from '@/features/categories/pages/categories';
import { BankConnectionsPage } from '@/features/bank-connections/pages/bank-connections';
import { BankCallbackPage } from '@/features/bank-connections/pages/bank-callback';

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 60 * 1000,
      retry: 1,
    },
  },
});

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route element={<ProtectedRoute />}>
              <Route element={<AppLayout />}>
                <Route path="/" element={<DashboardPage />} />
                <Route path="/accounts" element={<Navigate to="/bank-connections" />} />
                <Route path="/transactions" element={<TransactionsPage />} />
                <Route path="/categories" element={<CategoriesPage />} />
                <Route path="/bank-connections" element={<BankConnectionsPage />} />
                <Route path="/bank-connections/callback" element={<BankCallbackPage />} />
              </Route>
            </Route>
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
          <Toaster />
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
}
