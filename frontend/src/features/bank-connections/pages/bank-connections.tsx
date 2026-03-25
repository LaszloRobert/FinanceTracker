import { useState } from 'react';
import {
  Landmark,
  Plus,
  ExternalLink,
  ChevronDown,
  RefreshCw,
  Wallet,
} from 'lucide-react';
import { useBankConnections, useInstitutions, useConnectBank } from '../hooks/use-bank-connections';
import { useAccounts } from '@/features/accounts/hooks/use-accounts';
import { useSyncAccount, useSyncAll } from '@/features/transactions/hooks/use-sync-transactions';
import { Button } from '@/components/ui/button';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from '@/components/ui/card';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from '@/components/ui/collapsible';

export function BankConnectionsPage() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [expandedId, setExpandedId] = useState<string | null>(null);

  const { data: connections, isLoading } = useBankConnections();
  const { data: allAccounts } = useAccounts();
  const { data: institutions, isLoading: institutionsLoading } = useInstitutions('RO', dialogOpen);
  const connectMutation = useConnectBank();
  const syncAccount = useSyncAccount();
  const syncAll = useSyncAll();

  const handleConnect = (institutionId: string) => {
    connectMutation.mutate(institutionId, {
      onSuccess: () => setDialogOpen(false),
    });
  };

  const getAccountsForConnection = (connectionId: string) =>
    allAccounts?.filter((a) => a.bankConnectionId === connectionId) ?? [];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Bank Connections</h1>
          <p className="text-muted-foreground">
            Manage your banks and accounts.
          </p>
        </div>
        <div className="flex gap-2">
          {connections && connections.length > 0 && (
            <Button
              variant="outline"
              onClick={() => syncAll.mutate()}
              disabled={syncAll.isPending}
            >
              <RefreshCw className={`size-4 ${syncAll.isPending ? 'animate-spin' : ''}`} />
              {syncAll.isPending ? 'Syncing...' : 'Sync All'}
            </Button>
          )}
          <Button onClick={() => setDialogOpen(true)}>
            <Plus className="size-4" />
            Connect Bank
          </Button>
        </div>
      </div>

      {/* Loading */}
      {isLoading && (
        <p className="text-sm text-muted-foreground">Loading connections...</p>
      )}

      {/* Empty state */}
      {connections && connections.length === 0 && (
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-16">
            <div className="mb-4 flex size-14 items-center justify-center rounded-full bg-muted">
              <Landmark className="size-7 text-muted-foreground" />
            </div>
            <p className="text-sm font-medium">No bank connections yet</p>
            <p className="mb-4 text-xs text-muted-foreground">
              Connect your first bank to start tracking transactions.
            </p>
            <Button onClick={() => setDialogOpen(true)} size="sm">
              <Plus className="size-4" />
              Connect Bank
            </Button>
          </CardContent>
        </Card>
      )}

      {/* Bank connection cards */}
      {connections && connections.length > 0 && (
        <div className="space-y-3">
          {connections.map((connection) => {
            const accounts = getAccountsForConnection(connection.id);
            const isExpanded = expandedId === connection.id;

            return (
              <Collapsible
                key={connection.id}
                open={isExpanded}
                onOpenChange={(open) => setExpandedId(open ? connection.id : null)}
              >
                <Card>
                  <CollapsibleTrigger className="w-full text-left">
                    <CardHeader className="cursor-pointer transition-colors hover:bg-muted/50">
                      <div className="flex items-center justify-between">
                        <div className="flex items-center gap-3">
                          {connection.institutionLogo ? (
                            <img
                              src={connection.institutionLogo}
                              alt={connection.institutionName}
                              className="size-10 rounded-lg border bg-white object-contain p-1"
                            />
                          ) : (
                            <div className="flex size-10 items-center justify-center rounded-lg border bg-muted">
                              <Landmark className="size-5 text-muted-foreground" />
                            </div>
                          )}
                          <div>
                            <CardTitle className="text-base">
                              {connection.institutionName}
                            </CardTitle>
                            <CardDescription className="flex items-center gap-2">
                              <span
                                className={`inline-block size-2 rounded-full ${
                                  connection.status === 'Linked'
                                    ? 'bg-emerald-500'
                                    : 'bg-amber-500'
                                }`}
                              />
                              {connection.status}
                              <span className="text-muted-foreground">
                                &middot; {accounts.length} account{accounts.length !== 1 ? 's' : ''}
                              </span>
                              <span className="text-muted-foreground">
                                &middot; Expires {new Date(connection.expiresAt).toLocaleDateString()}
                              </span>
                            </CardDescription>
                          </div>
                        </div>

                        <ChevronDown
                          className={`size-5 text-muted-foreground transition-transform duration-200 ${
                            isExpanded ? 'rotate-180' : ''
                          }`}
                        />
                      </div>
                    </CardHeader>
                  </CollapsibleTrigger>

                  <CollapsibleContent>
                    <CardContent className="border-t pt-4">
                      {accounts.length === 0 && (
                        <p className="py-4 text-center text-sm text-muted-foreground">
                          No accounts found for this connection.
                        </p>
                      )}

                      {accounts.length > 0 && (
                        <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                          {accounts.map((account) => (
                            <div
                              key={account.id}
                              className="flex flex-col gap-2 rounded-lg border p-4"
                            >
                              <div className="flex items-start justify-between">
                                <div className="flex items-center gap-2">
                                  <div className="flex size-8 items-center justify-center rounded-md bg-muted">
                                    <Wallet className="size-4 text-muted-foreground" />
                                  </div>
                                  <div>
                                    <p className="text-sm font-medium">
                                      {account.displayName ?? account.currency}
                                    </p>
                                    <p className="text-xs text-muted-foreground">
                                      {account.iban ?? 'No IBAN'}
                                    </p>
                                  </div>
                                </div>
                                <Button
                                  variant="ghost"
                                  size="icon"
                                  className="size-8"
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    syncAccount.mutate(account.id);
                                  }}
                                  disabled={syncAccount.isPending}
                                >
                                  <RefreshCw
                                    className={`size-3.5 ${syncAccount.isPending ? 'animate-spin' : ''}`}
                                  />
                                </Button>
                              </div>

                              <div className="space-y-1 text-xs text-muted-foreground">
                                {account.ownerName && (
                                  <div className="flex justify-between">
                                    <span>Owner</span>
                                    <span className="font-medium text-foreground">
                                      {account.ownerName}
                                    </span>
                                  </div>
                                )}
                                <div className="flex justify-between">
                                  <span>Currency</span>
                                  <span className="font-medium text-foreground">
                                    {account.currency}
                                  </span>
                                </div>
                                {account.lastSyncedAt && (
                                  <div className="flex justify-between">
                                    <span>Last synced</span>
                                    <span>{new Date(account.lastSyncedAt).toLocaleString()}</span>
                                  </div>
                                )}
                              </div>
                            </div>
                          ))}
                        </div>
                      )}
                    </CardContent>
                  </CollapsibleContent>
                </Card>
              </Collapsible>
            );
          })}
        </div>
      )}

      {/* Connect bank dialog */}
      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="max-h-[80vh] overflow-y-auto sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Connect a Bank</DialogTitle>
            <DialogDescription>
              Select your banking institution to get started.
            </DialogDescription>
          </DialogHeader>

          {institutionsLoading && (
            <p className="py-4 text-center text-sm text-muted-foreground">
              Loading institutions...
            </p>
          )}

          {institutions && (
            <div className="space-y-1">
              {institutions.map((institution) => (
                <button
                  key={institution.id}
                  onClick={() => handleConnect(institution.id)}
                  disabled={connectMutation.isPending}
                  className="flex w-full cursor-pointer items-center gap-3 rounded-lg border p-3 text-left transition-colors hover:bg-muted disabled:opacity-50"
                >
                  {institution.logo ? (
                    <img
                      src={institution.logo}
                      alt={institution.name}
                      className="size-8 rounded object-contain"
                    />
                  ) : (
                    <div className="flex size-8 items-center justify-center rounded bg-muted">
                      <Landmark className="size-4 text-muted-foreground" />
                    </div>
                  )}
                  <span className="flex-1 text-sm font-medium">{institution.name}</span>
                  <ExternalLink className="size-4 text-muted-foreground" />
                </button>
              ))}
            </div>
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
