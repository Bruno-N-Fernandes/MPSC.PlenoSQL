Drop Table Configuracao;
Drop Table Sessao;
Drop Table Tipo;
PRAGMA foreign_keys = ON;

Create Table Sessao (
        Id Integer Not Null Primary Key AutoIncrement,
        Nome Varchar(50) Not Null
);

Create Table Tipo (
        Id Integer Not Null Primary Key AutoIncrement,
        Nome Varchar(50) Not Null,
        Valores Varchar(250) Not Null
);



Create Table Configuracao (
        Id Integer Not Null Primary Key AutoIncrement,
        SessaoId Integer Not Null References Sessao(Id),
        TipoId Integer Not Null References Tipo(Id),
        Nome Varchar(50) Not Null,
        Valor Varchar(250) Not Null
);

Insert Into Sessao (Nome) Values ('Sistema');
Insert Into Sessao (Nome) Values ('Preferencias');
Insert Into Tipo (Nome, Valores) Values ('Inteiro', 'd');
Insert Into Tipo (Nome, Valores) Values ('Logico', '0|1');


Insert Into Configuracao (SessaoId, TipoId, Nome, Valor) values (1, 1, 'SalvarAoExecutar', 'Teste');

Select c.rowid,
    --C.Id,
    S.Nome As Sessao,
    C.Nome As Config,
    T.Nome As Tipo,
    C.Valor
From Configuracao C
Inner Join Sessao S On S.Id = C.SessaoId
Inner Join Tipo T On T.Id = C.TipoId
;

Select * From Configuracao c as c..SessaoIdNome

SELECT * FROM 
WHERE tbl_name = 'table_name' AND type = 'table'


select * from (select "") left join Configuracao b on -1 = b.rowid;

SELECT * FROM sqlite_master
WHERE tbl_name = 'Configuracao' AND type = 'table'


;;



WITH
    Recordify(tbl_name, Ordinal, Clause, Sql)
AS
    (
     SELECT
        tbl_name,
        0,

        '',
        Sql
     FROM
        (
         SELECT
            tbl_name,
            substr
            (
             Sql,
             instr(Sql, '(') + 1,
             length(Sql) - instr(Sql, '(') - 1
            ) || ',' Sql
         FROM
            sqlite_master
         WHERE
            type = 'table'
        )
     UNION ALL
     SELECT
        tbl_name,
        Ordinal + 1,
        trim(substr(Sql, 1, instr(Sql, ',') - 1)),
        substr(Sql, instr(Sql, ',') + 1)
     FROM
        Recordify
     WHERE
        Sql > ''
       AND  lower(trim(Sql)) NOT LIKE 'check%'
       AND  lower(trim(Sql)) NOT LIKE 'unique%'
       AND  lower(trim(Sql)) NOT LIKE 'primary%'
       AND  lower(trim(Sql)) NOT LIKE 'foreign%'
       AND  lower(trim(Sql)) NOT LIKE 'constraint%'
    ),
    -- Added to make querying a subset easier.
    Listing(tbl_name, Ordinal, Name, Constraints)
AS
    (
     SELECT
        tbl_name,
        Ordinal,
        substr(Clause, 1, instr(Clause, ' ') - 1),
        trim(substr(Clause, instr(Clause, ' ') + 1))
     FROM
        Recordify
     WHERE
        Ordinal > 0
    )
SELECT
    tbl_name,
    Ordinal,
    Name,
    Constraints
FROM
    Listing
ORDER BY
    tbl_name,
    lower(Name);
    
    
select * from USUARIO u where u.
    
    
    