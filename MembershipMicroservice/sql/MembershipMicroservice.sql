DROP TABLE IF EXISTS membership;

CREATE TABLE membership (
    id SMALLSERIAL PRIMARY KEY,
    created_at TIMESTAMP NULL,
    last_modification TIMESTAMP NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,

    created_by VARCHAR(150),
    modified_by VARCHAR(150),

    name VARCHAR(200) NULL,
    price DECIMAL(10,2) NULL,
    description TEXT NULL,
    monthly_sessions SMALLINT NULL
);

INSERT INTO membership (created_at, name, price, description, monthly_sessions, created_by)
VALUES
    (now(), 'Fit Básica',        120.00, 'Acceso general y 8 sesiones guiadas al mes.',            8,  'system'),
    (now(), 'Clásica',           180.00, '12 sesiones mensuales, acceso a peso libre y cardio.',  12, 'system'),
    (now(), 'Full Acceso',       260.00, '20 sesiones + clases funcionales y spinning.',          20, 'system'),
    (now(), 'Premium Ilimitada', 320.00, 'Clases y salas ilimitadas, incluye invitado 1 vez/mes.',30, 'system'),
    (now(), 'Plan Estudiante',    95.00, 'Tarifa reducida con 10 sesiones al mes.',               10, 'system');

SELECT * FROM membership;
