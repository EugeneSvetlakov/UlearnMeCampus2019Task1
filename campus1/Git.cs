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

        /// <summary>
        /// Словарь отслеживаемых файлов
        /// </summary>
        Dictionary<int, Dictionary<int, int>> _Files;
        
        /// <summary>
        /// Список файлов на обновление/добавление
        /// </summary>
        private readonly Dictionary<int, int> _ToUpdateList;

        public Git(int filesCount)
        {
            if (IsOutOfFileCountRange(filesCount))
                throw new ArgumentException("Files Count out of supported range!");

            _Files = new Dictionary<int, Dictionary<int, int>>();

            _ToUpdateList = new Dictionary<int, int>();

            this._filesCount = filesCount;
        }

        public void Update(int fileNumber, int value)
        {
            if (IsOutOfFileNumRange(fileNumber))
                throw new ArgumentException("File number out of existing range!");

            if (_ToUpdateList.Count >= _maxCount)
                throw new ArgumentException("MaxUpdateCount assigned");

            _ToUpdateList[fileNumber] = value;
        }

        public int Commit()
        {
            if (IsOutOfCommitRange(_CommitCount + 1))
                throw new ArgumentException("Commits MaxSize assigned");

            _CommitCount++;

            if (_ToUpdateList.Count == 0)
                return _CommitCount;

            foreach (var item in _ToUpdateList)
            {
                _Files[item.Key][_CommitCount] = item.Value;
            }

            _ToUpdateList.Clear();

            return _CommitCount;
        }

        public int Checkout(int commitNumber, int fileNumber)
        {
            if (_CommitCount == -1)
                throw new ArgumentException("Out of Commit range");
            if (IsOutOfCommitRange(commitNumber))
                throw new ArgumentException("Out of Commit range");

            if (commitNumber > _CommitCount)
                throw new ArgumentException("There is no such Commit");

            if (IsOutOfFileNumRange(fileNumber))
                throw new ArgumentException("Out of fileNumbers Range");

            if (!_Files.ContainsKey(fileNumber))
                return 0;
                //throw new ArgumentException("No such file");

            var firstCommit = _Files[fileNumber].Keys.First();
            var lastCommit = _Files[fileNumber].Keys.Last();

            if (firstCommit > commitNumber)
                throw new ArgumentException("File not yet commited");

            if (_Files[fileNumber].ContainsKey(commitNumber))
                return _Files[fileNumber][commitNumber];

            return _Files[fileNumber][lastCommit];
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