using Dooggy.LIBRARY;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dooggy.CORE
{
    public class DataTag : List<OptionTag>
    {
        public myDominio Dominio;

        public string name => Dominio.name;
        public string padrao => Dominio.padrao;

        public string value;

        public string log => Dominio.log;  //GetLOG();


        public bool IsMatch(string prmName) => myString.IsEqual(name, prmName);
        public bool IsPadrao(string prmValue) => myString.IsEqual(padrao, prmValue);

        public DataTag()
        {
        }
        public DataTag(DataTag prmTag)
        {
            Parse(prmTag.Dominio);
        }
        public DataTag(myDominio prmDominio)
        {
            Parse(prmDominio);
        }
        public void Parse(myDominio prmDominio)
        {
            Dominio = prmDominio;

            foreach (string item in prmDominio.Opcoes)
                Add(new OptionTag(prmValue: item, this));


        }

        public void SetAtivado(string prmOption, bool prmAtivo)
        {
            foreach (OptionTag Option in this)
                if (Option.IsMatch(prmOption))
                { Option.SetAtivo(prmAtivo); break; }
        }

        public bool GetAtivado(string prmTag, string prmOption)
        {
            foreach (OptionTag Option in this)
                if (Option.IsMatch(prmTag, prmOption))
                    return true;
            return false;
        }

    }

    public class DataTags : List<DataTag>
    {

        public DataTags Todos => GetTodos();
        public DataTags Ativos => GetTodos(prmFiltrar: true);

        public void Parse(DataTags prmTags)
        {
            foreach (DataTag Tag in prmTags)
                AddItem(Tag.Dominio);
        }

        private void AddItem(myDominio prmTag) => base.Add(new DataTag(prmTag));

        public void SetAtivado(string prmName, string prmOption, bool prmAtivo)
        {
            foreach (DataTag Tag in this)

                if (Tag.IsMatch(prmName))
                { Tag.SetAtivado(prmOption, prmAtivo); break; }
        }

        private DataTags GetTodos() => GetTodos(prmFiltrar: false);
        private DataTags GetTodos(bool prmFiltrar)
        {
            DataTags itens = new DataTags();

            foreach (DataTag Tag in this)
            {
                foreach (OptionTag item in Tag)
                    if (item.ativo || !prmFiltrar)
                        itens.Add(item);
            }
            return itens;
        }

    }

    public class OptionTag
    {

        public DataTag Tag;

        public string value;

        public bool ativo;

        private string name => Tag.Dominio.name;
        public string log => String.Format("{0}: {1}", name, value);

        public bool IsPadrao => Tag.Dominio.IsPadrao(value);

        public bool IsMatch(string prmName, string prmValue) => IsMatchName(prmName) && IsMatchValue(prmValue);
        public bool IsMatch(string prmValue) => IsMatchValue(prmValue);

        private bool IsMatchName(string prmName) => myString.IsEqual(name, prmName);
        private bool IsMatchValue(string prmValue) => myString.IsEqual(value, prmValue);

        public OptionTag(string prmValue, DataTag prmTag)
        {
            Tag = prmTag;

            value = prmValue;
        }
        public void SetAtivo(bool prmAtivo) => ativo = prmAtivo;

    }

    public class OptionsTag : List<OptionTag>
    {

        public myDominio Dominio;

    }
}
