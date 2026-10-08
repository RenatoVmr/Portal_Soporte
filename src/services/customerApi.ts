export type AccountStatus = "ACTIVE" | "BLOCKED";

export interface CustomerAccount {
  customerId: number;
  documentNumber: string;
  businessName: string;
  accountId: number;
  accountNumber: string;
  status: AccountStatus;
  statusReason: string | null;
}

const API_URL = "http://localhost:5016";

export async function getCustomers(): Promise<CustomerAccount[]> {
  const response = await fetch(`${API_URL}/api/customers`);

  if (!response.ok) {
    throw new Error("Could not load customers.");
  }

  return response.json();
}

export async function updateAccountStatus(
  accountId: number,
  status: AccountStatus,
  statusReason: string
): Promise<void> {
  const response = await fetch(
    `${API_URL}/api/accounts/${accountId}/status`,
    {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ status, statusReason }),
    }
  );

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message ?? "Could not update account status.");
  }
}