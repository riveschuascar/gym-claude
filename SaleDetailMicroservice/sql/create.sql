BEGIN;

-- DROP OLD TABLES
DROP TABLE IF EXISTS sale_details;

-- Ejecutar en SaleDetailMicroserviceDB

CREATE TABLE sale_details (
    id SERIAL PRIMARY KEY,
    sale_id INTEGER NOT NULL, -- Referencia lógica al ID del otro microservicio
    discipline_id INTEGER NOT NULL,
    qty INTEGER NOT NULL DEFAULT 1,
    price NUMERIC(10,2) NOT NULL,
    total NUMERIC(10,2) NOT NULL,
    start_date DATE,
    end_date DATE
);

CREATE INDEX idx_sale_details_sale_id ON sale_details(sale_id);

INSERT INTO sale_details (sale_id, discipline_id, qty, price, total, start_date, end_date)
VALUES
    (1, 1, 1, 120.00, 120.00, current_date, current_date),
    (2, 3, 2, 130.00, 260.00, current_date, current_date),
    (3, 5, 1, 95.00, 95.00, current_date, current_date);
COMMIT;
