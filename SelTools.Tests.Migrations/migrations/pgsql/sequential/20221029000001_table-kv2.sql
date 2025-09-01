CREATE TABLE kv2 (
     id              SERIAL PRIMARY KEY  NOT NULL
    ,created_ts      TIMESTAMP           NOT NULL DEFAULT CURRENT_TIMESTAMP
    ,modified_ts     TIMESTAMP           NOT NULL DEFAULT CURRENT_TIMESTAMP
    ,"key"           TEXT                NOT NULL
    ,"value"         TEXT                NOT NULL
    ,UNIQUE ("key")
);

