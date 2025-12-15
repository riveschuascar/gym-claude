--BEGIN;

--DROP TABLE IF EXISTS outbox_messages;
--DROP TABLE IF EXISTS membership_sale;

--CREATE TABLE membership_sale (
--    id SERIAL PRIMARY KEY,
--    client_id INTEGER NOT NULL,
--    membership_id INTEGER NOT NULL,
--    start_date DATE NOT NULL,
--    end_date DATE NOT NULL,
--    sale_date DATE NOT NULL DEFAULT CURRENT_DATE,
--    total_amount NUMERIC(10,2) NOT NULL,
--    payment_method VARCHAR(50) NOT NULL,
--    tax_id VARCHAR(50),
--    business_name VARCHAR(200),
--    notes TEXT,
--    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
--    last_modification TIMESTAMPTZ,
--    is_active BOOLEAN NOT NULL DEFAULT TRUE,
--    created_by VARCHAR(150),
--    modified_by VARCHAR(150),
--    CONSTRAINT chk_dates CHECK (end_date >= start_date),
--    CONSTRAINT chk_amount CHECK (total_amount > 0)
--);

--CREATE INDEX idx_membership_sale_active ON membership_sale(is_active);
--CREATE INDEX idx_membership_sale_client ON membership_sale(client_id);
--CREATE INDEX idx_membership_sale_membership ON membership_sale(membership_id);

--CREATE TABLE outbox_messages (
--    id UUID PRIMARY KEY,
--    message_type VARCHAR(150) NOT NULL,
--    payload JSONB NOT NULL,
--    occurred_on TIMESTAMPTZ NOT NULL,
--    correlation_id VARCHAR(150),
--    operation_id VARCHAR(150)
--);

--CREATE INDEX idx_outbox_messages_pending ON outbox_messages(occurred_on);

---- Datos de ejemplo (asume clientes 1..3, membresías 1..5 según seeds)
--INSERT INTO membership_sale
--    (client_id, membership_id, start_date, end_date, sale_date, total_amount, payment_method, tax_id, business_name, notes, created_by)
--VALUES
--    (1, 1, current_date, current_date, current_date, 120.00, 'Efectivo',      '12345678', 'Juan Perez', 'Venta presencial', 'system'),
--    (2, 3, current_date, current_date, current_date, 260.00, 'Tarjeta',       '98765432', 'Maria Condori', 'Incluye functional y spinning', 'system'),
--    (3, 5, current_date, current_date, current_date, 95.00,  'Transferencia', '45678912', 'Luis Apaza', 'Tarifa estudiante', 'system');

--COMMIT;
