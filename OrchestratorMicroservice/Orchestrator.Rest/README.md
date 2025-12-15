# Orchestrator REST

This microservice receives a `sale-created` event and orchestrates the following flow:

- Validate client by calling `GET {ClientsBaseUrl}/api/clients/{clientId}` (200 -> valid, 404 -> invalid)
- For each discipline id, call `POST {DisciplinesBaseUrl}/api/disciplines/validate` with body `{ id: <disciplineId> }` and expect `{ validated: true|false }`
- If all validations succeed, call `POST {SalesBaseUrl}/api/sales/{saleId}/complete` to mark sale as completed and send status `{ status: "Completed", reason: "Orchestration completed" }`
- If validation fails (client or discipline), call `POST {SalesBaseUrl}/api/sales/{saleId}/status` with a status like `ClientNotFound` or `DisciplineFailed` and an explanatory `reason` field
- If the sale is marked, notify reports with `POST {ReportsBaseUrl}/api/reports/sales` and body `{ saleId: <saleId> }`

Sample request to the orchestrator:

```json
{
  "IdOrquestas": 1,
  "SaleId": 123,
  "ClientId": 42,
  "DisciplinesIds": [10, 11, 12]
}
```

Configuration is in `appsettings.json` under `Services` (base URLs for each dependent service).
