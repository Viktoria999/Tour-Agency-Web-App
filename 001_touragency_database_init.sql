create schema auth;
create schema catalog;
create schema log;


create table auth.user
(
    id                  uuid        not null
        primary key,
    username            varchar(50) not null,
    password            text        not null,
    first_name          varchar(50),
    last_name           varchar(50),
    middle_name         varchar(50),
    email               text,
    profile_picture_url text,
    is_admin            boolean
);

alter table auth.user
    owner to postgres;

create function auth.user_edit(_id uuid, _username character varying, _password text, _first_name character varying DEFAULT NULL::character varying(50), _last_name character varying DEFAULT NULL::character varying(50), _middle_name character varying DEFAULT NULL::character varying(50), _email text DEFAULT NULL::text, _profile_picture_url text DEFAULT NULL::text) returns void
    language plpgsql
as
$$
BEGIN
    UPDATE auth.user
    SET
        username = _username,
        password = _password,
        first_name = _first_name,
        last_name = _last_name,
        middle_name = _middle_name,
        email = _email,
        profile_picture_url = _profile_picture_url
    WHERE _id = id;
END;
$$;

alter function auth.user_edit(uuid, varchar, text, varchar, varchar, varchar, text, text) owner to postgres;

create function auth.user_get(_username character varying)
    returns TABLE(user_id uuid, user_username character varying, user_password text, user_first_name character varying, user_last_name character varying, user_middle_name character varying, user_email text, user_profile_picture_url text, user_isadmin boolean)
    language plpgsql
as
$$
BEGIN
    RETURN QUERY
        SELECT
            id as user_id,
            username as user_username,
            password as user_password,
            first_name as user_first_name,
            last_name as user_last_name,
            middle_name as user_middle_name,
            email as user_email,
            profile_picture_url as user_profile_picture_url,
            is_admin as user_is_admin
        FROM auth.user
        WHERE username = _username;
END;
$$;

alter function auth.user_get(varchar) owner to postgres;

create function auth.user_insert(_id uuid, _username character varying, _password text, _first_name character varying DEFAULT NULL::character varying(50), _last_name character varying DEFAULT NULL::character varying(50), _middle_name character varying DEFAULT NULL::character varying(50), _email text DEFAULT NULL::text, _profile_picture_url text DEFAULT NULL::text) returns void
    language plpgsql
as
$$
BEGIN
    IF EXISTS (SELECT 1 FROM auth.user WHERE username = _username) THEN
        RAISE EXCEPTION 'Имя пользователя "%" уже существует', _username;
    END IF;

    IF EXISTS (SELECT 1 FROM auth.user WHERE email = _email) THEN
        RAISE EXCEPTION 'Адрес эл.почты "%" уже занят', _email;
    END IF;

    INSERT INTO auth.user
    (
        id,
        username,
        password,
        first_name,
        last_name,
        middle_name,
        email,
        profile_picture_url
    )
    VALUES
        (
            _id,
            _username,
            _password,
            _first_name,
            _last_name,
            _middle_name,
            _email,
            _profile_picture_url
        );
END;
$$;

alter function auth.user_insert(uuid, varchar, text, varchar, varchar, varchar, text, text) owner to postgres;

create table catalog.items
(
    id               serial
        primary key,
    name             text not null,
    description      text not null,
    url              text not null,
    city             varchar(50),
    star_rating      integer,
    beach_type       varchar(50),
    is_all_inclusive boolean
);

alter table catalog.items
    owner to postgres;

create function catalog.items_select(_id integer DEFAULT NULL::integer, _city character varying DEFAULT NULL::character varying(50), _beach_type character varying DEFAULT NULL::character varying(50), _star_rating integer DEFAULT NULL::integer, _is_all_inclusive boolean DEFAULT NULL::boolean)
    returns TABLE(item_id integer, item_name text, item_description text, item_url text, item_city character varying, item_beach_type character varying, item_star_rating integer, item_is_all_inclusive boolean)
    language plpgsql
as
$$
BEGIN
    RETURN QUERY
        SELECT
            id as item_id,
            name as item_name,
            description as item_description,
            url as item_url,
            city as item_city,
            beach_type as item_beach_type,
            star_rating as item_star_rating,
            is_all_inclusive as item_is_all_inclusive
        FROM
            catalog.items
        WHERE
            (_id IS NULL OR id = _id) AND
            (_city IS NULL OR city = _city) AND
            (_beach_type IS NULL OR beach_type = _beach_type) AND
            (_star_rating IS NULL OR star_rating = _star_rating) AND
            (_is_all_inclusive IS NULL OR is_all_inclusive = _is_all_inclusive);
END;
$$;

alter function catalog.items_select(integer, varchar, varchar, integer, boolean) owner to postgres;

create function catalog.item_insert(_name text DEFAULT NULL::text, _description text DEFAULT NULL::text, _url text DEFAULT NULL::text, _city character varying DEFAULT NULL::character varying(50), _star_rating integer DEFAULT NULL::integer, _beach_type character varying DEFAULT NULL::character varying(50), _is_all_inclusive boolean DEFAULT NULL::boolean) returns void
    language plpgsql
as
$$
BEGIN
    INSERT INTO catalog.items
    (
        name,
        description,
        url,
        city,
        star_rating,
        beach_type,
        is_all_inclusive
    )
    VALUES
        (
            _name,
            _description,
            _url,
            _city,
            _star_rating,
            _beach_type,
            _is_all_inclusive
        );
END;
$$;

alter function catalog.item_insert(text, text, text, varchar, integer, varchar, boolean) owner to postgres;

create function catalog.item_edit(_id integer, _name text DEFAULT NULL::text, _description text DEFAULT NULL::text, _url text DEFAULT NULL::text, _city character varying DEFAULT NULL::character varying(50), _star_rating integer DEFAULT NULL::integer, _beach_type character varying DEFAULT NULL::character varying(50), _is_all_inclusive boolean DEFAULT NULL::boolean) returns void
    language plpgsql
as
$$
BEGIN
    UPDATE catalog.items
    SET
        name = _name,
        description = _description,
        url = _url,
        city = _city,
        star_rating = _star_rating,
        beach_type = _beach_type,
        is_all_inclusive = _is_all_inclusive
    WHERE id = _id;
END;
$$;

alter function catalog.item_edit(integer, text, text, text, varchar, integer, varchar, boolean) owner to postgres;

create table log.logs
(
    id         serial
        primary key,
    controller text,
    action     text,
    message    text,
    level      text,
    log_time   timestamp,
    username   text
);

alter table log.logs
    owner to postgres;

create function log.logs_get_popular_pages_rating()
    returns TABLE(log_controller text, log_action text, log_request_count bigint)
    language plpgsql
as
$$
BEGIN
    RETURN QUERY
        select logs.controller as log_controller,
               logs.action as log_action,
               count(logs.id) as log_request_count
        from log.logs
        group by log_controller, log_action
        order by log_request_count desc
        limit 10;
END;
$$;

alter function log.logs_get_popular_pages_rating() owner to postgres;

create function log.logs_select(_id integer DEFAULT NULL::integer, _controller text DEFAULT NULL::text, _action text DEFAULT NULL::text, _message text DEFAULT NULL::text, _level text DEFAULT NULL::text, _time_from timestamp without time zone DEFAULT NULL::timestamp without time zone, _time_to timestamp without time zone DEFAULT NULL::timestamp without time zone, _username text DEFAULT NULL::text, _period text DEFAULT NULL::text)
    returns TABLE(log_id integer, log_controller text, log_action text, log_message text, log_level text, log_log_time timestamp without time zone, log_username text)
    language plpgsql
as
$$
BEGIN
    RETURN QUERY
        SELECT
            id as log_id,
            controller as log_controller,
            action as log_action,
            message as log_message,
            level as log_level,
            log_time as log_log_time,
            username as log_username
        FROM
            log.logs
        WHERE
            (_id IS NULL OR id = _id) AND
            (_controller IS NULL OR controller = _controller) AND
            (_action IS NULL OR action = _action) AND
            (_message IS NULL OR message = _message) AND
            (_level IS NULL OR level = _level) AND
            (_username IS NULL OR username = _username) AND
            (_time_from IS NULL OR log_time >= _time_from) AND
            (_time_to IS NULL OR log_time <= _time_to) AND
            (
                _period IS NULL OR
                (_period = 'day' AND log_time >= CURRENT_TIMESTAMP - INTERVAL '1 day') OR
                (_period = 'week' AND log_time >= CURRENT_TIMESTAMP - INTERVAL '1 week') OR
                (_period = 'month' AND log_time >= CURRENT_TIMESTAMP - INTERVAL '1 month') OR
                (_period = 'year' AND log_time >= CURRENT_TIMESTAMP - INTERVAL '1 year')
                );
END;
$$;

alter function log.logs_select(integer, text, text, text, text, timestamp, timestamp, text, text) owner to postgres;

