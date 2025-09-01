CREATE TABLE kv (
     id              INT PRIMARY KEY
    ,created_ts      TIMESTAMP           NOT NULL DEFAULT CURRENT_TIMESTAMP
    ,modified_ts     TIMESTAMP           NOT NULL DEFAULT CURRENT_TIMESTAMP
    ,"key"           TEXT                NOT NULL
    ,"value"         TEXT                NOT NULL
    ,UNIQUE ("key")
);

