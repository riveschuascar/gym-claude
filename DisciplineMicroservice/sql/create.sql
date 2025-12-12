BEGIN;

DROP TABLE IF EXISTS public.discipline;

CREATE TABLE public.discipline (
    id                  SMALLSERIAL PRIMARY KEY,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    last_modification   TIMESTAMPTZ,
    is_active           BOOLEAN NOT NULL DEFAULT TRUE,

    created_by          VARCHAR(150),
    modified_by         VARCHAR(150),

    name                VARCHAR(50) NOT NULL,

    id_user             BIGINT,

    start_time          INTERVAL NOT NULL,
    end_time            INTERVAL NOT NULL,

    CONSTRAINT chk_discipline_name_chars CHECK (
        name ~ '^[[:alpha:][:digit:] ''-]+$'
    ),

    CONSTRAINT chk_discipline_letters CHECK (
        length(regexp_replace(name, '[^[:alpha:]]', '', 'g')) >= 3
    ),

    CONSTRAINT chk_discipline_digits CHECK (
        length(regexp_replace(name, '[^[:digit:]]', '', 'g')) <= 2
    ),

    CONSTRAINT chk_discipline_letters_if_digits CHECK (
        length(regexp_replace(name, '[^[:digit:]]', '', 'g')) = 0
        OR length(regexp_replace(name, '[^[:alpha:]]', '', 'g')) >= 5
    ),

    CONSTRAINT chk_discipline_time_order CHECK (
        end_time > start_time
    ),

    CONSTRAINT chk_discipline_duration_min CHECK (
        end_time - start_time >= INTERVAL '1 hour'
    ),

    CONSTRAINT chk_discipline_duration_max CHECK (
        end_time - start_time <= INTERVAL '2 hour'
    ),

    CONSTRAINT chk_discipline_opening_hours CHECK (
        start_time >= INTERVAL '06:00' AND 
        end_time   <= INTERVAL '22:00'
    )
);

CREATE INDEX idx_discipline_is_active ON public.discipline(is_active);
CREATE INDEX idx_discipline_name      ON public.discipline(name);

INSERT INTO public.discipline
    (name, id_user, start_time, end_time, created_by)
VALUES
    ('Funcional AM',     1, '07:00', '08:30', 'system'),
    ('Spinning After',   2, '18:00', '19:00', 'system'),
    ('Yoga Flow',        3, '17:00', '18:00', 'system'),
    ('CrossFit Base',    4, '19:00', '20:30', 'system'),
    ('Pilates Core',     5, '09:00', '10:00', 'system'),
    ('Boxeo Tecnico',    6, '20:30', '21:30', 'system'),
    ('Zumba Cardio',     7, '10:30', '11:30', 'system'),
    ('HIIT Express',     8, '12:30', '13:30', 'system');

COMMIT;

-- Ajustar horario permitido en tablas existentes (06:00-22:00)
ALTER TABLE public.discipline
    DROP CONSTRAINT IF EXISTS chk_discipline_opening_hours;

ALTER TABLE public.discipline
    ADD CONSTRAINT chk_discipline_opening_hours
        CHECK (start_time >= INTERVAL '06:00' AND end_time <= INTERVAL '22:00');
