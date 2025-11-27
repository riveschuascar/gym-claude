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
    (now(), 'Basic Plan',    100.00, 'Acceso limitado al gimnasio.',       12, 'system'),
    (now(), 'Premium Plan',  200.00, 'Acceso total y clases ilimitadas.',  30, 'system'),
    (now(), 'Student Plan',   80.00, 'Precio especial para estudiantes.',   8, 'system');

SELECT * FROM membership;
