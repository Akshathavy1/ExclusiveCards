﻿using ExclusiveCard.Enums;

namespace ExclusiveCard.Website.Models
{

    public class TransactionLogRequest
    {
        private TransactionLogSortField _transactionSortField;

        public TransactionLogSortField TransactionSortField
        {
            get { return _transactionSortField; }
            set { _transactionSortField = (TransactionLogSortField)value; }
        }

        private string _sortDirection = "asc";
        public string SortDirection
        {
            get
            {
                return _sortDirection;
            }
            set { if (!(string.IsNullOrEmpty(value))) { _sortDirection = value; } }
        }

        private string _sorticon = "fa fa-sort-alpha-asc";

        public string SortIcon
        {
            get
            {
                return _sorticon;
            }
            set { if (!(string.IsNullOrEmpty(value))) { _sorticon = value; } }
        }
    }
}
