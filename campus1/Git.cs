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
            if (_CommitCount == -1)
                return 0;
            
            // Запрашиваемый Коммит вне диаппазона: 0 <= commitNumber < 50000
            if (IsOutOfCommitRange(commitNumber))
                throw new ArgumentException("Out of Commit range");
            
            // Коммита еще не было
            if (commitNumber > _CommitCount)
                throw new ArgumentException("There is no such Commit");

            // Номер файла вне диаппазона: 0..(_filesCount-1)
            if (IsOutOfFileNumRange(fileNumber))
                throw new ArgumentException("Out of fileNumbers Range");

            Dictionary<int, int> FilesState = GetFiles(commitNumber);

            return FilesState[fileNumber];
        }

        private Dictionary<int, int> GetFiles(int commitNumber)
        {
            Dictionary<int, int> FilesOnCommit = new Dictionary<int, int>();
            foreach (var item in _Files)
            {
                FilesOnCommit.Add(item.Key, item.Value);
            }

            var listKeys = _Files.Keys.ToList();
            var listCommitKey = _Commits.Keys.ToList();

            do
            {
                listCommitKey.Remove(listKeys.Last());
            } while (listKeys.Count > 0 || listCommitKey.Count > 0);

            return FilesOnCommit;
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