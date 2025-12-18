BEGIN;

-- DROP OLD TABLES
DROP TABLE IF EXISTS outbox_messages;
DROP TABLE IF EXISTS membership_sale;
DROP TABLE IF EXISTS sale_details;
DROP TABLE IF EXISTS sales;

-- New sales table
CREATE TABLE sales (
    id SERIAL PRIMARY KEY,
    client_id INTEGER NOT NULL,
    sale_date DATE NOT NULL DEFAULT CURRENT_DATE,
    total_amount NUMERIC(10,2) NOT NULL,
    nit VARCHAR(50),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    last_modification TIMESTAMPTZ,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_by VARCHAR(150),
    modified_by VARCHAR(150),
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    CONSTRAINT chk_amount CHECK (total_amount > 0)
);

CREATE INDEX idx_sales_active ON sales(is_active);
CREATE INDEX idx_sales_client ON sales(client_id);

-- Example data (assume clients 1..3 and disciplines 1..5)
INSERT INTO sales (client_id, sale_date, total_amount, nit, created_by, status)
VALUES
    (1, current_date, 120.00, '12345678', 'system', 'Completed'),
    (2, current_date, 260.00, '98765432', 'system', 'Completed'),
    (3, current_date, 95.00,  '45678912', 'system', 'Completed');