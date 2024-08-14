using System;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        // DDL de exemplo
        string ddl = @"
            CREATE TABLE eldados_transparencia_geral.dbo.vw_portal_gmp_compra_ordem (
                id int IDENTITY(1,1) NOT NULL,
                portal_id int NOT NULL,
                hash_registro nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                transparencia_hash_cliente nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
                tipo_processo nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_tipo_processo nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_tipo_af nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_empresa nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_empresa nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                cnpj_empresa nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_filial nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_filial nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                cnpj_filial nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                descricao_licitacao nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                descricao_contrato nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_ordem_compra nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                data_ordem_compra date NULL,
                nome_modalidade nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                numero_ordem_compra nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                ano_ordem_compra nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_situacao nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_secretaria nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_secretaria nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_local_requerente nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_local_requerente nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_g_fornecedor nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_g_fornecedor nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_cpf_cnpj nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                cpf_cnpj nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                numero_processo nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                ano_processo nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                descricao_ordco nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_licitacao nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_contrato nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_motivo nvarchar(400) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                vlr_total numeric(25,9) NULL,
                ano nvarchar(4) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                mes nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                anexo nvarchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS DEFAULT N'N' NULL,
                esfera_administrativa int NULL,
                esfera_poder varchar(1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                esfera_poder_tipo varchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                esfera_poder_ug varchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_tce varchar(15) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                numero_artigo varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                codigo_local_entrega varchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                nome_local_entrega nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                hash_registro_area nvarchar(38) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                controle_area nvarchar(60) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
                CONSTRAINT pk_vw_portal_gmp_compra_ordem PRIMARY KEY (id)
            );
        ";

        // Extrair informações e gerar código
        string configurationCode = GenerateEntityConfiguration(ddl);
        Console.WriteLine(configurationCode);
    }

    static string GenerateEntityConfiguration(string ddl)
    {
        var output = new System.Text.StringBuilder();

        // Ajuste a regex para lidar com o COLLATE
        var matches = Regex.Matches(ddl, @"(\w+)\s+(\w+)(\(\d+,\d+\)|\(\d+\))?(?:\s+COLLATE\s+\w+)?\s+(NOT\s+NULL|NULL)?(DEFAULT\s+[^\s,]+)?(,\s*|$)");

        foreach (Match match in matches)
        {
            string columnName = match.Groups[1].Value;
            string columnType = match.Groups[2].Value.ToLower();
            string length = match.Groups[3].Value;
            bool isRequired = match.Groups[4].Value.Contains("NOT NULL");

            // Ignorar linhas desnecessárias
            if (columnName.Equals("COLLATE", StringComparison.OrdinalIgnoreCase))
                continue;

            string propertyName = ConvertToPascalCase(columnName);

            output.AppendLine($"builder.Property(e => e.{propertyName})");
            output.AppendLine($"    .HasColumnName(\"{columnName}\")");

            // Verifica o tipo e adiciona o comprimento se necessário
            if (!string.IsNullOrEmpty(length))
            {
                if (columnType == "nvarchar")
                {
                    // Converter nvarchar para varchar
                    output.AppendLine($"    .HasColumnType(\"varchar{length}\")");
                }
                else if (columnType == "varchar")
                {
                    output.AppendLine($"    .HasColumnType(\"varchar{length}\")");
                }
                else
                {
                    output.AppendLine($"    .HasColumnType(\"{columnType}{length}\")");
                }
            }
            else
            {
                output.AppendLine($"    .HasColumnType(\"{columnType}\")");
            }

            output.AppendLine($"    .IsRequired({isRequired.ToString().ToLower()});");
            output.AppendLine();
        }

        return output.ToString();
    }

    static string ConvertToPascalCase(string columnName)
    {
        var words = columnName.Split('_');
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
        return string.Join("", words);
    }
}
