-- Tabla de relacion membresia - disciplina
CREATE TABLE IF NOT EXISTS membership_discipline (
    id serial PRIMARY KEY,
    membership_id integer NOT NULL,
    discipline_id integer NOT NULL,
    created_at timestamptz NOT NULL DEFAULT now(),
    last_modification timestamptz NOT NULL DEFAULT now(),
    is_active boolean NOT NULL DEFAULT true,
    created_by varchar(200),
    modified_by varchar(200)
);

-- Evita duplicados activos
CREATE UNIQUE INDEX IF NOT EXISTS ux_membership_discipline_active
    ON membership_discipline (membership_id, discipline_id)
    WHERE is_active = true;

-- Limpia datos previos para evitar duplicados al re-ejecutar seeds
TRUNCATE TABLE membership_discipline RESTART IDENTITY;

-- Datos de prueba coherentes con los seeds de discipline y membership:
-- memberships:
-- 1 Fit Básica, 2 Clásica, 3 Full Acceso, 4 Premium Ilimitada, 5 Plan Estudiante
-- disciplines (ids según inserción en discipline.sql):
-- 1 Funcional AM, 2 Spinning After, 3 Yoga Flow, 4 CrossFit Base,
-- 5 Pilates Core, 6 Boxeo Tecnico, 7 Zumba Cardio, 8 HIIT Express
INSERT INTO membership_discipline (membership_id, discipline_id, created_by)
VALUES
    (1, 1, 'system'), -- Fit Básica con Funcional AM
    (1, 5, 'system'), -- Fit Básica con Pilates Core
    (2, 1, 'system'), -- Clásica con Funcional AM
    (2, 5, 'system'), -- Clásica con Pilates Core
    (2, 7, 'system'), -- Clásica con Zumba
    (3, 1, 'system'), -- Full Acceso con Funcional
    (3, 2, 'system'), -- Full Acceso con Spinning
    (3, 3, 'system'), -- Full Acceso con Yoga
    (3, 7, 'system'), -- Full Acceso con Zumba
    (3, 8, 'system'), -- Full Acceso con HIIT
    (4, 1, 'system'), -- Premium con Funcional
    (4, 2, 'system'), -- Premium con Spinning
    (4, 3, 'system'), -- Premium con Yoga
    (4, 4, 'system'), -- Premium con CrossFit
    (4, 5, 'system'), -- Premium con Pilates
    (4, 6, 'system'), -- Premium con Boxeo
    (4, 7, 'system'), -- Premium con Zumba
    (4, 8, 'system'), -- Premium con HIIT
    (5, 3, 'system'), -- Estudiante con Yoga
    (5, 7, 'system'), -- Estudiante con Zumba
    (5, 8, 'system'); -- Estudiante con HIIT
