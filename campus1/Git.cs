using System;
using System.Collections.Generic;
using System.Linq;

namespace GitTask
{
    public class Git
    {
        private readonly int? _filesCount;

        private readonly int _maxCount = 50000;
        private readonly int _minFilesCount = 1;
        private int _CommitCount = -1;

        Dictionary<int, int> _Files;
        Dictionary<int, int> _Updates;
        Dictionary<int, Dictionary<int, int>> _Commits;

        public int CommitCount { get => _CommitCount; set => _CommitCount = value; }

        public Git(int filesCount)
        {
            if (IsOutOfFileCountRange(filesCount))
                throw new ArgumentException("Files Count out of supported range!");

            _Files = new Dictionary<int, int>();

            for (int i = 0; i < filesCount; i++)
            {
                _Files.Add(i, 0);
            }

            _Commits = new Dictionary<int, Dictionary<int, int>>();

            _Updates = new Dictionary<int, int>();

            this._filesCount = filesCount;
        }

        public void Update(int fileNumber, int value)
        {
            if (IsOutOfFileNumRange(fileNumber))
                throw new ArgumentException("File number out of existing range!");

            _Updates[fileNumber] = value;
        }

        public int Commit()
        {
            if (IsOutOfCommitRange(_CommitCount + 1))
                throw new ArgumentException("Commits MaxSize assigned");

            _CommitCount++;
            _Commits.Add(_CommitCount, new Dictionary<int, int>());

            if (_Updates.Count == 0)
                return _CommitCount;

            foreach (var item in _Updates)
            {
                _Commits[_CommitCount].Add(item.Key, item.Value);
            }

            return _CommitCount;
        }

        public int Checkout(int commitNumber, int fileNumber)
        {
            // не было комитов
            if (_CommitCount < 0)
                return _Files[fileNumber];

            // Запрашиваемый Коммит вне диаппазона: 0 <= commitNumber < 50000
            if (IsOutOfCommitRange(commitNumber))
                throw new ArgumentException("Out of Commit range");

            // Коммита еще не было
            if (commitNumber > _CommitCount)
                throw new ArgumentException("There is no such Commit");

            //
            if(!_Commits.ContainsKey(commitNumber))
                throw new ArgumentException("There is no such Commit");

            // Номер файла вне диаппазона: 0..(_filesCount-1)
            if (IsOutOfFileNumRange(fileNumber))
                throw new ArgumentException("Out of fileNumbers Range");

            var data = GetActualDataFromFile(fileNumber, commitNumber);

            return data;
        }

        /// <summary>
        /// Актуальные значения всех файлов 
        /// </summary>
        /// <param name="commitNumber"></param>
        /// <returns></returns>
        private Dictionary<int, int> GetFiles(int commitNumber)
        {
            Dictionary<int, int> FilesOnCommit = new Dictionary<int, int>();
            foreach (var item in _Files)
            {
                FilesOnCommit.Add(item.Key, item.Value);
            }

            var listFileNums = _Files.Keys.ToList();
            var listCommitKey = _Commits
                .Where(c => c.Key < commitNumber)
                .ToDictionary(d => d.Key, d => d.Value)
                .Keys.ToList();

            for (int comitKey = listCommitKey.Count - 1; comitKey >= 0; comitKey--)
            {
                var commit = _Commits[listCommitKey.ElementAt(comitKey)];
                var keysInComit = commit.Keys.Select(k => listFileNums.FirstOrDefault(el => el == k)).ToList();

                foreach (var fileKey in keysInComit)
                {
                    FilesOnCommit[fileKey] = commit[fileKey];
                }

                listFileNums.RemoveAll(e => keysInComit.Contains(e));
                if (listFileNums.Count == 0) break;
            }

            return FilesOnCommit;
        }

        /// <summary>
        /// Поиск файла по всем коммитам, если там нет, то берем из _Files
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <returns></returns>
        private int GetActualDataFromFile(int fileNumber, int commitNumber)
        {
            bool hasFileInCommit = _Commits[commitNumber].ContainsKey(fileNumber);

            if (hasFileInCommit)
                return _Commits[commitNumber][fileNumber];

            var commit = _Commits
                .Where(c => c.Key < commitNumber)
                .ToDictionary(d => d.Key, d => d.Value)
                .LastOrDefault(c => c.Value.ContainsKey(fileNumber));

            if (commit.Equals(default(KeyValuePair<int, Dictionary<int, int>>)))
                if (commit.Value != null)
                    if(commit.Value.Equals(default(Dictionary<int,int>)))
                        return commit.Value[fileNumber];

            return _Files[fileNumber];
        }

        private bool IsOutOfFileNumRange(int fileNumber)
        {
            return (fileNumber < _minFilesCount - 1 || fileNumber > _filesCount - 1);
        }

        private bool IsOutOfCommitRange(int commitNumber)
        {
            return (commitNumber < _minFilesCount - 1 || commitNumber > _maxCount - 1);
        }

        private bool IsOutOfFileCountRange(int filesCount)
        {
            return (filesCount < _minFilesCount || filesCount > _maxCount);
        }
    }
}