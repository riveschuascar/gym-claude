# Gym Claude – Notas de la base para la saga de ventas

Este repo ya tiene los microservicios separados. Se agregó una base para la transacción distribuida de ventas y la UI se enriqueció para alinearse con la rúbrica (búsquedas y correlación).

## Cambios clave
- **SalesMicroservice** ahora emite un evento de dominio `VentaCreada` en outbox (`outbox_messages`) después de crear una venta. Tabla y script: `SalesMicroservice/sql/create.sql`.
- **Outbox**: se guarda como JSON (`message_type`, `payload`, `occurred_on`, `correlation_id`, `operation_id`). El worker/publicador queda pendiente.
- **Clientes HTTP (stubs)** en Sales para validar existencia de Cliente y Membresía (`IClientApi`, `IMembershipApi`). Usan `HttpClient` con timeout configurable y propagan el JWT + `X-Correlation-Id`/`X-Operation-Id`.
- **Correlación**: Sales genera `CorrelationId`/`OperationId` si no vienen y los devuelve en headers. WebUI (Ventas) los envía en cada alta.
- **UI de ventas** (`WebUI/Pages/Sales/Create`):
  - Buscadores de clientes y membresías con filtros.
  - Links a crear cliente/membresía si no existen.
  - Resumen dinámico (cliente, CI/NIT, membresía, sesiones, total).
  - Genera y envía `X-Correlation-Id` y `X-Operation-Id` al Sales API.

## Configuración nueva
- `SalesMicroservice/SalesMicroserviceAPI/appsettings*.json` agrega:
  - `ExternalApis:Client` y `ExternalApis:Membership` (URLs base).
  - `ExternalApis:TimeoutSeconds` para los `HttpClient`.

## Headers y trazabilidad
- `X-Correlation-Id`: se usa para agrupar la transacción distribuida; se genera si no viene.
- `X-Operation-Id`: id específico de la operación (por defecto igual al `CorrelationId`).
- Ambos se propagan de WebUI → Sales y de Sales → otros microservicios vía `PropagationDelegatingHandler`.

## Puntos de extensión pendientes (equipo)
1) **Publicador Outbox**: worker/cron que lea `outbox_messages` y publique en el bus (RabbitMQ/Service Bus). Manejar reintentos e idempotencia.
2) **Saga**: orquestar/coreografiar la venta (validación cruzada real, reportes, email, etc.) consumiendo el evento `VentaCreada`.
3) **Reporte de venta**: microservicio de reportes que escuche `VentaCreada` y genere el comprobante PDF automáticamente; opcional: enviar email.
4) **Auditoría**: poblar `CreatedBy/ModifiedBy` con claims JWT en Sales y resto de servicios; usar `X-Correlation-Id` en logs.
5) **Validaciones reales**: implementar llamadas de `ClientApi` y `MembershipApi` (ahora stubs optimistas) y manejar fallos de red/respuestas 404 según reglas de negocio.
6) **Idempotencia/Outbox Plus**: agregar marca de idempotencia en el handler de eventos y limpiar procesados.

## Flujo resumido (alta de venta)
1) WebUI (Ventas/Create) genera `CorrelationId` y `OperationId`, permite buscar/seleccionar cliente y membresía, autocompleta total.
2) Envía POST a `Sales API /api/Sales` con JWT + headers de correlación.
3) Sales valida entrada + existencia (stubs), persiste venta en `membership_sale`, construye `VentaCreada` y la guarda en outbox.
4) Sales responde devolviendo los headers de correlación para trazabilidad.
5) Pendiente: worker publica desde outbox, microservicio de reportes consume y emite comprobante.
