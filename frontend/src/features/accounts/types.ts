export interface Account {
  id: string;
  bankConnectionId: string;
  iban: string | null;
  currency: string;
  ownerName: string | null;
  displayName: string | null;
  product: string | null;
  lastSyncedAt: string | null;
}
