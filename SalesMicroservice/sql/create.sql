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
    CONSTRAINT chk_amount CHECK (total_amount > 0)
);

CREATE INDEX idx_sales_active ON sales(is_active);
CREATE INDEX idx_sales_client ON sales(client_id);

-- New sale_details table
CREATE TABLE sale_details (
    id SERIAL PRIMARY KEY,
    sale_id INTEGER NOT NULL REFERENCES sales(id) ON DELETE CASCADE,
    discipline_id INTEGER NOT NULL,
    qty INTEGER NOT NULL DEFAULT 1,
    price NUMERIC(10,2) NOT NULL,
    total NUMERIC(10,2) NOT NULL,
    start_date DATE,
    end_date DATE
);

CREATE INDEX idx_sale_details_sale ON sale_details(sale_id);
CREATE INDEX idx_sale_details_discipline ON sale_details(discipline_id);

-- Example data (assume clients 1..3 and disciplines 1..5)
INSERT INTO sales (client_id, sale_date, total_amount, nit, created_by)
VALUES
    (1, current_date, 120.00, '12345678', 'system'),
    (2, current_date, 260.00, '98765432', 'system'),
    (3, current_date, 95.00,  '45678912', 'system');

INSERT INTO sale_details (sale_id, discipline_id, qty, price, total, start_date, end_date)
VALUES
    (1, 1, 1, 120.00, 120.00, current_date, current_date),
    (2, 3, 2, 130.00, 260.00, current_date, current_date),
    (3, 5, 1, 95.00, 95.00, current_date, current_date);

COMMIT;
