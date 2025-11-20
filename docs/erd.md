erDiagram

    COMPANIES ||--o{ USERS : has
    COMPANIES ||--o{ EMPLOYEES : employs
    USERS ||--o{ COMPLIANCE_QUESTIONS : asks
    COMPANIES ||--o{ COMPLIANCE_QUESTIONS : has

    COMPANIES ||--o{ ROSTER_CHECKS : has
    ROSTER_CHECKS ||--o{ ROSTER_ISSUES : generates
    EMPLOYEES ||--o{ ROSTER_ISSUES : affected

    COMPANIES ||--o{ DOCUMENT_TEMPLATES : owns
    DOCUMENT_TEMPLATES ||--o{ DOCUMENTS : generate_from
    EMPLOYEES ||--o{ DOCUMENTS : assigned

    COMPANIES ||--o{ PAYRUNS : has
    PAYRUNS ||--o{ PAYRUN_ISSUES : has
    EMPLOYEES ||--o{ PAYRUN_ISSUES : affected

    COMPANIES ||--o{ STP_CHECKS : has
    STP_CHECKS ||--o{ STP_ISSUES : results
    EMPLOYEES ||--o{ STP_ISSUES : related

    USERS ||--o{ EMPLOYEE_QA_LOGS : submits
    EMPLOYEES ||--o{ EMPLOYEE_QA_LOGS : linked

    USERS {
        uuid id PK
        uuid company_id FK
        string email
        string password_hash
        string role   // admin, hr, manager, payroll, employee
        datetime created_at
    }

    COMPANIES {
        uuid id PK
        string name
        string abn
        string default_award_code
        datetime created_at
    }

    EMPLOYEES {
        uuid id PK
        uuid company_id FK
        uuid user_id FK nullable
        string first_name
        string last_name
        string employment_type // ft, pt, casual
        string award_code
        numeric base_rate
        string status
        datetime created_at
    }

    COMPLIANCE_QUESTIONS {
        uuid id PK
        uuid company_id FK
        uuid user_id FK
        text question_text
        string award_code
        string state
        string risk_level
        text answer_summary
        json next_steps
        json references
        text disclaimer
        datetime created_at
    }

    ROSTER_CHECKS {
        uuid id PK
        uuid company_id FK
        uuid user_id FK
        date period_start
        date period_end
        string status
        string overall_risk_level
        numeric estimated_extra_cost
        datetime created_at
    }

    ROSTER_ISSUES {
        uuid id PK
        uuid roster_check_id FK
        uuid employee_id FK
        date date
        time shift_start
        time shift_end
        string issue_type
        text description
        numeric estimated_cost_impact
    }

    DOCUMENT_TEMPLATES {
        uuid id PK
        uuid company_id FK nullable
        string type
        string employment_type
        string name
        text content
        datetime last_updated_at
    }

    DOCUMENTS {
        uuid id PK
        uuid company_id FK
        uuid employee_id FK
        uuid template_id FK
        string type
        string status
        text content
        numeric sg_rate
        json warnings
        datetime created_at
    }

    PAYRUNS {
        uuid id PK
        uuid company_id FK
        date period_start
        date period_end
        string source
        string status
        int total_employees
        datetime created_at
    }

    PAYRUN_ISSUES {
        uuid id PK
        uuid payrun_id FK
        uuid employee_id FK
        string issue_type
        string risk_level
        text description
        numeric estimated_underpay_amount
    }

    STP_CHECKS {
        uuid id PK
        uuid company_id FK
        datetime created_at
        string status
        int total_employees
        int invalid_count
    }

    STP_ISSUES {
        uuid id PK
        uuid stp_check_id FK
        uuid employee_id FK
        string field_name
        string current_value
        text issue
    }

    EMPLOYEE_QA_LOGS {
        uuid id PK
        uuid user_id FK
        uuid employee_id FK
        text question
        text answer
        string risk_level
        datetime created_at
    }
