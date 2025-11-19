-- Script único para inicializar y poblar la tabla de clientes
-- Motor: PostgreSQL
-- Uso: ejecutar este archivo en la base de datos usada por el microservicio de clientes.

BEGIN;

-- Eliminar tabla anterior si existe (útil para entornos de desarrollo)
DROP TABLE IF EXISTS public.client;

-- Crear tabla principal de clientes
CREATE TABLE public.client (
    id                      SERIAL PRIMARY KEY,
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_modification       TIMESTAMPTZ,
    is_active               BOOLEAN NOT NULL DEFAULT TRUE,
    name                    VARCHAR(200),
    first_lastname          VARCHAR(200),
    second_lastname         VARCHAR(200),
    date_birth              DATE,
    ci                      VARCHAR(50),
    fitness_level           VARCHAR(50) DEFAULT 'Intermedio',
    initial_weight_kg       NUMERIC(10,2),
    current_weight_kg       NUMERIC(10,2),
    emergency_contact_phone VARCHAR(50),
    CONSTRAINT chk_client_fitness_level_es
        CHECK (
            fitness_level IS NULL
            OR fitness_level IN ('Principiante', 'Intermedio', 'Avanzado')
        )
);

-- Índices útiles
CREATE INDEX idx_client_is_active ON public.client(is_active);
CREATE INDEX idx_client_name      ON public.client(name);

-- Datos de ejemplo para probar el CRUD desde la WebUI
INSERT INTO public.client
    (name, first_lastname, second_lastname, date_birth, ci, is_active, created_at,
     fitness_level, initial_weight_kg, current_weight_kg, emergency_contact_phone)
VALUES
    ('Juan',    'Pérez',     'López',      DATE '1990-01-01', 'C-123456', TRUE, NOW(), 'Principiante',  82.50, 82.50, '777-111'),
    ('María',   'González',  'Rojas',      DATE '1988-05-12', 'C-234567', TRUE, NOW(), 'Intermedio',    68.20, 67.00, '777-222'),
    ('Carlos',  'Ramírez',   'Soto',       DATE '1995-09-23', 'C-345678', TRUE, NOW(), 'Avanzado',      90.00, 88.30, '777-333'),
    ('Ana',     'Fernández', 'Díaz',       DATE '1992-03-04', 'C-456789', TRUE, NOW(), 'Principiante',  55.10, 54.80, '777-444'),
    ('Luis',    'Torres',    'Mendoza',    DATE '1985-12-31', 'C-567890', TRUE, NOW(), 'Intermedio',    76.00, 75.20, '777-555'),
    ('Sofía',   'Rivas',     'Ortega',     DATE '1998-07-15', 'C-678901', TRUE, NOW(), 'Avanzado',      62.00, 61.50, '777-666'),
    ('Miguel',  'Castro',    'Vega',       DATE '1993-11-20', 'C-789012', TRUE, NOW(), 'Principiante',  85.00, 84.50, '777-777'),
    ('Lucía',   'Navarro',   'Ibarra',     DATE '1991-04-18', 'C-890123', TRUE, NOW(), 'Intermedio',    58.00, 57.60, '777-888'),
    ('Diego',   'Salazar',   'Quispe',     DATE '1996-10-10', 'C-901234', TRUE, NOW(), 'Avanzado',      92.40, 91.00, '777-999'),
    ('Valeria', 'Campos',    'Paredes',    DATE '1999-02-27', 'C-012345', TRUE, NOW(), 'Principiante',  64.30, 63.90, '777-000');

COMMIT;

