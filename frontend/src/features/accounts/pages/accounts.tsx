import { Wallet, RefreshCw } from 'lucide-react';
import { useAccounts } from '../hooks/use-accounts';
import { useSyncAccount, useSyncAll } from '@/features/transactions/hooks/use-sync-transactions';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';

export function AccountsPage() {
  const { data: accounts, isLoading } = useAccounts();
  const syncAccount = useSyncAccount();
  const syncAll = useSyncAll();

  return (
    <div className="space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Accounts</h1>
          <p className="text-muted-foreground">Your connected bank accounts.</p>
        </div>
        {accounts && accounts.length > 0 && (
          <Button
            onClick={() => syncAll.mutate()}
            disabled={syncAll.isPending}
          >
            <RefreshCw className={`size-4 ${syncAll.isPending ? 'animate-spin' : ''}`} />
            {syncAll.isPending ? 'Syncing...' : 'Sync All'}
          </Button>
        )}
      </div>

      {isLoading && (
        <p className="text-sm text-muted-foreground">Loading accounts...</p>
      )}

      {accounts && accounts.length === 0 && (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12">
            <Wallet className="mb-3 size-10 text-muted-foreground" />
            <p className="text-sm font-medium">No accounts yet</p>
            <p className="text-xs text-muted-foreground">
              Connect a bank to import your accounts.
            </p>
          </CardContent>
        </Card>
      )}

      {accounts && accounts.length > 0 && (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {accounts.map((account) => (
            <Card key={account.id}>
              <CardHeader>
                <div className="flex items-center justify-between">
                  <div>
                    <CardTitle>{account.displayName ?? account.ownerName ?? 'Account'}</CardTitle>
                    <CardDescription>{account.iban}</CardDescription>
                  </div>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => syncAccount.mutate(account.id)}
                    disabled={syncAccount.isPending}
                  >
                    <RefreshCw className={`size-4 ${syncAccount.isPending ? 'animate-spin' : ''}`} />
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="flex items-center justify-between text-sm">
                  <span className="text-muted-foreground">Currency</span>
                  <span className="font-medium">{account.currency}</span>
                </div>
                {account.ownerName && (
                  <div className="mt-1 flex items-center justify-between text-sm">
                    <span className="text-muted-foreground">Owner</span>
                    <span className="font-medium">{account.ownerName}</span>
                  </div>
                )}
                {account.lastSyncedAt && (
                  <div className="mt-1 flex items-center justify-between text-sm">
                    <span className="text-muted-foreground">Last synced</span>
                    <span className="text-xs text-muted-foreground">
                      {new Date(account.lastSyncedAt).toLocaleString()}
                    </span>
                  </div>
                )}
              </CardContent>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
