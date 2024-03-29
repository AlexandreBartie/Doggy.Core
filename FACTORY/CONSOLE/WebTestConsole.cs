﻿using Katty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Dooggy
{
    public class TestConsole
    {

        public TestFactory Factory;

        public TestConsoleInput Input;

        public TestConsoleOutput Output;

        public TestConsoleConfig Config;

        public TestScripts Scripts;

        public DataPool Pool => Factory.Pool;
        public TestTrace Trace => Factory.Trace;
        public DataSource Dados => Pool.Dados;

        public DataConnect Connect => Pool.Connect;
        public DataBases Bases => Pool.Bases;

        public TestConfigPath Path => Config.Path;

        public TestResult Result => Script.Result;
        public TestScript Script { get => Scripts.Corrente; }

        public bool IsOK => Config.IsOK;
        public bool IsDbOK => Connect.IsDbOK;
        private bool IsScript => (Script != null);

        public string code_result { get { if (IsScript) return Result.code; return ""; } }
        public string data_result { get { if (IsScript) return Result.data; return ""; } }

        public TestConsole(TestFactory prmFactory)
        {

            Factory = prmFactory;

            Input = new TestConsoleInput(this);

            Output = new TestConsoleOutput(this);

            Config = new TestConsoleConfig(this);

            Scripts = new TestScripts(this);

        }

        public bool EXE(string prmArquivoCFG) => Setup(prmArquivoCFG, prmPlay: true);

        public bool Setup(string prmArquivoCFG) => Setup(prmArquivoCFG, prmPlay: false);
        public bool Setup(string prmArquivoCFG, bool prmPlay) => Config.Setup(prmArquivoCFG, prmPlay);

        public bool LoadCFG(string prmArquivoCFG) => Config.Load(prmArquivoCFG);

        public void SetAnchor(DateTime prmAncora) => Config.CSV.SetToday(prmAncora);

        public void SetDBStatus(bool prmBloqueado) => Config.Connect.SetDBStatus(prmBloqueado);

        public void AddLogItem() => Scripts.AddLogItem();
        public void AddLogSQL(string prmError) => Scripts.AddLogSQL(prmError);
        
        public void Load() => Load(prmPlay: false);
        public void Load(bool prmPlay) => Input.Load(prmPlay);

        public void Import(string prmArquivoINI) => LoadINI(prmArquivoINI, prmPlay: true);
        public void LoadINI(string prmArquivoINI) => LoadINI(prmArquivoINI, prmPlay: false);
        public void LoadINI(string prmArquivoINI, bool prmPlay) => Scripts.Load(prmArquivoINI, prmPlay);

        public void Play(string prmCode) => Play(prmCode, prmArquivoOUT: "");
        public void Play(string prmCode, string prmArquivoOUT) => Scripts.Play(prmCode, prmArquivoOUT);

        public bool SaveCode(string prmCode) => Input.SaveINI(prmCode);
        public void SetCode(string prmCode) => Script.SetCode(prmCode);
        public void UndoCode() => Script.Result.UndoCode();

        public bool SetScript(string prmKey) => Scripts.FindScript(prmKey);

        public bool DoConnect() => Dados.DoConnect();

    }
    public class TestConsoleInput : TestConsoleIO
    {

        public Arquivos GetArquivosINI() => Path.INI.Filtro("*.ini");

        public TestConsoleInput(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void Load(bool prmPlay)
        {
            foreach (Arquivo file in GetArquivosINI())
                Console.LoadINI(prmArquivoINI: file.nome_curto, prmPlay);
        }
        public bool SaveINI(string prmCode)
        {

            Result.SetSave(prmCode);

            return Dados.FileINI.Save(prmNome: GetNameScriptOrigem(), prmPath: Path.GetPathINI(), prmConteudo: prmCode);

        }

        public string GetCode(string prmArquivoINI) => Dados.FileINI.Open(prmArquivoINI,prmPath: Path.GetPathINI());

    }
    public class TestConsoleOutput : TestConsoleIO
    {

        public TestConsoleOutput(TestConsole prmConsole)
        {

            Console = prmConsole;

        }

        public void SaveOUT(string prmData, eTipoFileFormat prmTipo, string prmEncoding, string prmExtensao, string prmLog)
        {

            Dados.FileINI.Save(prmNome: GetNameScriptOrigem(), prmPath: Path.GetPathOUT(), prmConteudo: prmData, prmExtensao, prmEncoding);

            if (Mode.IsAutoPlay)
                Dados.FileLOG.Save(prmNome: GetNameScriptOrigem(), prmPath: Path.GetPathLOG(), prmConteudo: prmLog);

        }

    }
    public class TestConsoleIO
    {

        internal TestConsole Console;

        internal TestConfigMode Mode => Console.Config.Mode;
        internal TestConfigPath Path => Console.Config.Path;
        internal TestScript Script => Console.Script;
        internal TestResult Result => Script.Result;
        internal DataSource Dados => Console.Dados;
        internal TestTrace Trace => Console.Trace;

        internal string GetNameScriptOrigem()
        {

            string nome = Console.Result.name_OUT;

            if (myString.IsFull(nome))
                return (nome);

            return (Console.Result.name_INI);

        }

    }

}

