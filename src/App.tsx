import { useEffect, useState, type FormEvent } from "react";
import "./App.css";
import { getCustomers, updateAccountStatus } from "./services/customerApi";
import type { AccountStatus, CustomerAccount } from "./services/customerApi";

function App() {
  const [customers, setCustomers] = useState<CustomerAccount[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [selectedCustomer, setSelectedCustomer] =
    useState<CustomerAccount | null>(null);
  const [newStatus, setNewStatus] = useState<AccountStatus>("ACTIVE");
  const [reason, setReason] = useState("");
  const [saveError, setSaveError] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    getCustomers()
      .then(setCustomers)
      .catch((err: unknown) => {
        setError(err instanceof Error ? err.message : "Could not load customers.");
      })
      .finally(() => setLoading(false));
  }, []);

  const filteredCustomers = customers.filter((customer) =>
    customer.documentNumber.includes(search.trim())
  );

  function openEditor(customer: CustomerAccount) {
    setSelectedCustomer(customer);
    setNewStatus(customer.status);
    setReason(customer.statusReason ?? "");
    setSaveError("");
  }

  async function handleSave(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!selectedCustomer) return;

    setSaving(true);
    setSaveError("");

    try {
      await updateAccountStatus(
        selectedCustomer.accountId,
        newStatus,
        reason.trim()
      );

      const updatedCustomers = await getCustomers();
      setCustomers(updatedCustomers);
      setSelectedCustomer(null);
    } catch (err: unknown) {
      setSaveError(
        err instanceof Error ? err.message : "Could not update account status."
      );
    } finally {
      setSaving(false);
    }
  }

  return (
    <main className="page-shell">
      <header className="page-header">
        <div>
          <p className="eyebrow">Support Portal</p>
          <h1>Account management</h1>
          <p className="subtitle">
            Search for a customer and manage their account status.
          </p>
        </div>
      </header>

      <section className="content-card">
        <div className="toolbar">
          <div>
            <h2>Customers</h2>
            <p>{filteredCustomers.length} customer(s)</p>
          </div>

          <label className="search-field">
            <span>Search by document</span>
            <input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Enter document number"
            />
          </label>
        </div>

        {error && <p className="error-message">{error}</p>}

        <div className="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Document</th>
                <th>Business name</th>
                <th>Account</th>
                <th>Status</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={5}>Loading customers...</td>
                </tr>
              ) : filteredCustomers.length === 0 ? (
                <tr>
                  <td colSpan={5}>No customers found.</td>
                </tr>
              ) : (
                filteredCustomers.map((customer) => (
                  <tr key={customer.accountId}>
                    <td>{customer.documentNumber}</td>
                    <td>{customer.businessName}</td>
                    <td>{customer.accountNumber}</td>
                    <td>
                      <span
                        className={`status-pill ${customer.status.toLowerCase()}`}
                      >
                        {customer.status === "ACTIVE" ? "Active" : "Blocked"}
                      </span>
                    </td>
                    <td>
                      <button
                        className="edit-button"
                        onClick={() => openEditor(customer)}
                      >
                        Edit status
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </section>

      {selectedCustomer && (
        <div className="modal-backdrop">
          <section className="edit-panel">
            <button
              className="close-button"
              onClick={() => setSelectedCustomer(null)}
              aria-label="Close"
            >
              ×
            </button>

            <p className="eyebrow">Account status</p>
            <h2>{selectedCustomer.businessName}</h2>
            <p className="subtitle">
              Account {selectedCustomer.accountNumber}
            </p>

            <form onSubmit={handleSave}>
              <label className="form-field">
                <span>Status</span>
                <select
                  value={newStatus}
                  onChange={(event) =>
                    setNewStatus(event.target.value as AccountStatus)
                  }
                >
                  <option value="ACTIVE">Active</option>
                  <option value="BLOCKED">Blocked</option>
                </select>
              </label>

              {newStatus === "BLOCKED" && (
                <label className="form-field">
                  <span>Reason for blocking</span>
                  <textarea
                    value={reason}
                    onChange={(event) => setReason(event.target.value)}
                    placeholder="Enter the reason"
                    required
                  />
                </label>
              )}

              {saveError && <p className="error-message">{saveError}</p>}

              <button className="save-button" type="submit" disabled={saving}>
                {saving ? "Saving..." : "Save changes"}
              </button>
            </form>
          </section>
        </div>
      )}
    </main>
  );
}

export default App;