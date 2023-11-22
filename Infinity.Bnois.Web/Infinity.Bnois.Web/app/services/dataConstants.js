﻿
(function () {
    "use strict";
    angular.module("app").constant("dataConstants", {
        UPLOAD_FILE_URL: baseAPIUrl + 'api/bnois/upload-files/',
        DEPARTMENT_URL: baseAPIUrl + 'api/bnois/departments/',
        CATEGORY_URL: baseAPIUrl + 'api/bnois/categories/',
        COUNTRY_URL: baseAPIUrl + 'api/bnois/countries/',
        ORGANIZATION_URL: baseAPIUrl + 'api/bnois/organizations/',
        GENDER_URL: baseAPIUrl + 'api/bnois/genders/',
        DESIGNATION_URL: baseAPIUrl + 'api/bnois/designations/',
        RANK_URL: baseAPIUrl + 'api/bnois/ranks/',
        RELIGION_URL: baseAPIUrl + 'api/bnois/religions/',
        COURSE_URL: baseAPIUrl + 'api/bnois/courses/',
        LOCATION_URL: baseAPIUrl + 'api/bnois/locations/',
        TRAINING_TYPE_URL: baseAPIUrl + 'api/bnois/training-types/',
        TRAINING_URL: baseAPIUrl + 'api/bnois/trainings/',
        MINISTRY_URL: baseAPIUrl + 'api/bnois/ministries/',
        ENROLLMENT_URL: baseAPIUrl + 'api/bnois/enrollments/',
        FISCAL_YEAR_URL: baseAPIUrl + 'api/bnois/fiscal-years/',
        BUDGET_URL: baseAPIUrl + 'api/bnois/budgets/',
        ENROLLMENT_DETAIL_URL: baseAPIUrl + 'api/bnois/enrollment-details/',
        EMPLOYEE_CATEGORY_URL: baseAPIUrl + 'api/bnois/employee-categories/',
        BANK_URL: baseAPIUrl + 'api/bnois/banks/',
        CURRENCY_URL: baseAPIUrl + 'api/bnois/currencies/',
        EMPLOYEE_FAMILY_URL: baseAPIUrl + 'api/bnois/employee-families/',
        EMPLOYEE_RELATION_URL: baseAPIUrl + 'api/bnois/employee-relations/',
        BRANCH_URL: baseAPIUrl + 'api/bnois/branches/',
        BUDGET_ALLOCATION_URL: baseAPIUrl + 'api/bnois/budget-allocations/',
        MAIN_HEAD_URL: baseAPIUrl + 'api/bnois/main-heads/',
        ACCOUNT_HEAD_URL: baseAPIUrl + 'api/bnois/account-heads/',
        SUB_HEAD_URL: baseAPIUrl + 'api/bnois/sub-heads/',
        RANK_CATEGORY_URL: baseAPIUrl + 'api/bnois/rank-categories/',
        SUB_CATEGORY_URL: baseAPIUrl + 'api/bnois/sub-categories/',
        COMMISSION_TYPE_URL: baseAPIUrl + 'api/bnois/commission-types/',
        SUB_BRANCH_URL: baseAPIUrl + 'api/bnois/sub-branches/',
        OFFICER_STREAM_URL: baseAPIUrl + 'api/bnois/officer-streams/',
        BATCH_URL: baseAPIUrl + 'api/bnois/batches/',
        MARITAL_TYPE_URL: baseAPIUrl + 'api/bnois/marital-types/',
        SUBJECT_URL: baseAPIUrl + 'api/bnois/subjects/',
        EMPLOYEE_URL: baseAPIUrl + 'api/bnois/employees/',
        EMPLOYEEG_GENERAL_URL: baseAPIUrl + 'api/bnois/employee-generals/',
        DIVISION_URL: baseAPIUrl + 'api/bnois/divisions/',
		COLOR_URL: baseAPIUrl + 'api/bnois/colors/',
		PURPOSE_URL: baseAPIUrl + 'api/bnois/leave-purpose/',
        DISTRICT_URL: baseAPIUrl + 'api/bnois/districts/',
        MEDICAL_CATEGORY_URL: baseAPIUrl + 'api/bnois/medical-categories/',
        EYE_VISION_URL: baseAPIUrl + 'api/bnois/eye-visions/',
        BLOOD_GROUP_URL: baseAPIUrl + 'api/bnois/blood-groups/',
        PHYSICAL_STRUCTURE_URL: baseAPIUrl + 'api/bnois/physical-structures/',
        PHYSICAL_CONDITION_URL: baseAPIUrl + 'api/bnois/physical-conditions/',
        LEAVETYPE_URL: baseAPIUrl + 'api/bnois/leave-types/',
        LEAVEPOLICY_URL: baseAPIUrl + 'api/bnois/leavePolicy/',
        EMPLOYEEG_OTHER_URL: baseAPIUrl + 'api/bnois/employee-others/',
        UPAZILA_URL: baseAPIUrl + 'api/bnois/upazilas/',
        EXAM_CATEGORY_URL: baseAPIUrl + 'api/bnois/exam-categories/',
        EXAMINATION_URL: baseAPIUrl + 'api/bnois/examinations/',
        TERMINATION_TYPE_URL: baseAPIUrl + 'api/bnois/termination-types/',
        AGE_SERVICE_POLICY_URL: baseAPIUrl + 'api/bnois/age-service-policies/',
        INSTITUTE_TYPE_URL: baseAPIUrl + 'api/bnois/institute-types/',
        INSTITUTE_URL: baseAPIUrl + 'api/bnois/institutes/',
        RETIRED_AGE_URL: baseAPIUrl + 'api/bnois/retired-ages/',
        EMPLOYEE_SERVICE_EXTENSION_URL: baseAPIUrl + 'api/bnois/employee-service-extensions/',
        EMPLOYEELEAVE_URL: baseAPIUrl + 'api/bnois/employee-leave/',
        BOARD_URL: baseAPIUrl + 'api/bnois/boards/',
        COURSE_CATEGORY_URL: baseAPIUrl + 'api/bnois/course-categories/',
        COURSE_SUB_CATEGORY_URL: baseAPIUrl + 'api/bnois/course-sub-categories/',
        EMPLOYEE_RUN_MISSING_URL: baseAPIUrl + 'api/bnois/employee-run-missings/',
        EMPLOYEE_REJOIN_URL: baseAPIUrl + 'api/bnois/employee-rejoins/',
        EXAM_SUBJECT_URL: baseAPIUrl + 'api/bnois/exam-subjects/',
        RESULT_URL: baseAPIUrl + 'api/bnois/results/',
        RESULT_TYPE_URL: baseAPIUrl + 'api/bnois/result-types/',
        EDUCATION_URL: baseAPIUrl + 'api/bnois/educations/',
        TRAINING_INSTITUTE_URL: baseAPIUrl + 'api/bnois/training-institutes/',
        TRAINING_PLAN_URL: baseAPIUrl + 'api/bnois/training-plans/',
        ADDRESS_URL: baseAPIUrl + 'api/bnois/addresses/',
        TRAINING_RESULT_URL: baseAPIUrl + 'api/bnois/training-results/',
        SOCIAL_ATTRIBUTE_URL: baseAPIUrl + 'api/bnois/social-attributes/',
        EXTRACURRICULAR_TYPE_URL: baseAPIUrl + 'api/bnois/extracurricular-types/',
        EXTRACURRICULAR_URL: baseAPIUrl + 'api/bnois/extracurriculars/',
        EMPLOYEE_SPORT_URL: baseAPIUrl + 'api/bnois/employee-sports/',
        RELIGION_CAST_URL: baseAPIUrl + 'api/bnois/religion-casts/',
        OCCUPATION_URL: baseAPIUrl + 'api/bnois/occupations/',
        PARENT_URL: baseAPIUrl + 'api/bnois/parents/',
        SIBLING_URL: baseAPIUrl + 'api/bnois/siblings/',
        SPOUSE_URL: baseAPIUrl + 'api/bnois/spouses/',
        PRE_COMMISSION_RANK_URL: baseAPIUrl + 'api/bnois/pre-commission-ranks/',
        HEIR_TYPE_URL: baseAPIUrl + 'api/bnois/heir-types/',
        RELATION_URL: baseAPIUrl + 'api/bnois/relations/',
        Appointment_Nature_URL: baseAPIUrl + 'api/bnois/appointmentNature/',
        Appointment_Category_URL: baseAPIUrl + 'api/bnois/appointmentCategory/',
        HEIR_NEXT_OF_KIN_INFO_URL: baseAPIUrl + 'api/bnois/heir-next-of-kin-info/',
        Pattern_URL: baseAPIUrl + 'api/bnois/pattern/',
        PREVIOUS_EXPERIENCE_URL: baseAPIUrl + 'api/bnois/previous-experiences/',
        ZONE_URL: baseAPIUrl + 'api/bnois/zones/',
        SHIP_CATEGORY_URL: baseAPIUrl + 'api/bnois/ship-categories/',
        PROMOTION_BOARD_URL: baseAPIUrl + 'api/bnois/promotion-boards/',
        OFFICE_URL: baseAPIUrl + 'api/bnois/offices/',
        PHOTO_URL: baseAPIUrl + 'api/bnois/photos/',
        LPR_CALCULATE_INFO_URL: baseAPIUrl + 'api/bnois/Lpr-calculate-info/',
        EXECUTION_REMARK_URL: baseAPIUrl + 'api/bnois/execution-remarks/',
        OFFICE_APPOINTMENT_URL: baseAPIUrl + 'api/bnois/office-appointments/',
        PROMOTION_NOMINATION_URL: baseAPIUrl + 'api/bnois/promotion-nominations/',
        MEDAL_URL: baseAPIUrl + 'api/bnois/medals/',
        AWARD_URL: baseAPIUrl + 'api/bnois/awards/',
        PUBLICATION_CATEGORY_URL: baseAPIUrl + 'api/bnois/publication-categories/',
        PUBLICATION_URL: baseAPIUrl + 'api/bnois/publications/',
        MEMBER_Role_URL: baseAPIUrl + 'api/bnois/member-roles/',
        COMMENDATION_URL: baseAPIUrl + 'api/bnois/commendations/',
        PUNISHMENT_CATEGORY_URL: baseAPIUrl + 'api/bnois/punishment-categories/',
        PUNISHMENT_SUB_CATEGORY_URL: baseAPIUrl + 'api/bnois/punishment-sub-categories/',
        BOARD_MEMBER_URL: baseAPIUrl + 'api/bnois/board-members/',
        PUNISHMENT_NATURE_URL: baseAPIUrl + 'api/bnois/punishment-natures/',
        PROMOTION_EXECUTION_URL: baseAPIUrl + 'api/bnois/promotion-executions/',
        ACHIEVEMENT_URL: baseAPIUrl + 'api/bnois/achievements/',
		EMPLOYEE_LPR_URL: baseAPIUrl + 'api/bnois/employeelpr/',
		MEDAL_AWARD_URL: baseAPIUrl + 'api/bnois/medal-awards/',
		OBSERVATION_INTELLIGENT_URL: baseAPIUrl + 'api/bnois/observation-intelligent-reports/',
		PUNISHMENT_ACCIDENT_URL: baseAPIUrl + 'api/bnois/punishment-accidents/',
        LEAVE_BREAK_DOWN_URL: baseAPIUrl + 'api/bnois/leave-break-down/',
        NOMINATION_URL: baseAPIUrl + 'api/bnois/nominations/',
        NOMINATION_DETAIL_URL: baseAPIUrl + 'api/bnois/nomination-details/',
        PRE_COMMISSION_COURSES: baseAPIUrl + 'api/bnois/pre-commission-courses/',
        OFFICER_TRANSFERS: baseAPIUrl + 'api/bnois/officer-transfers/',
        NOMINATION_SCHEDULES_URL: baseAPIUrl + 'api/bnois/nomination-schedules/',
        VISIT_CATEGORIES_URL: baseAPIUrl + 'api/bnois/visit-categories/',
        VISIT_SUB_CATEGORIES_URL: baseAPIUrl + 'api/bnois/visit-sub-categories/',
        MISSION_APPOINTMENTS_URL: baseAPIUrl + 'api/bnois/mission-appointments/',
        SPORT_URL: baseAPIUrl + 'api/bnois/sports/',
        SPOUSE_FOREIGN_VISIT_URL: baseAPIUrl + 'api/bnois/spouse-foreign-visits/',
        PRE_COMMISSION_COURSE_DETAIL: baseAPIUrl + 'api/bnois/pre-commission-course-details/',
        EMPLOYEE_SECURITY_CLEARANCE_URL: baseAPIUrl + 'api/bnois/employee-security-clearances/',
        EMPLOYEE_FAMILY_PERMISSION_URL: baseAPIUrl + 'api/bnois/employee-family-permission/',
        EMPLOYEE_CAREER_FORECAST_URL: baseAPIUrl + 'api/bnois/employee-career-forecast/',
        SECURITY_CLEARANCE_REASON_URL: baseAPIUrl + 'api/bnois/security-clearance-reasons/',
        SERVICE_EXAM_CATEGORIES_URL: baseAPIUrl + 'api/bnois/service-exam-categories/',
        SERVICE_EXAM_URL: baseAPIUrl + 'api/bnois/service-exams/',
        EMPLOYEE_SERVICE_EXAM_RESULTS_URL: baseAPIUrl + 'api/bnois/employee-service-exam-results/',
        EMPLOYEE_PFTS_URL: baseAPIUrl + 'api/bnois/employee-pfts/',
        EMPLOYEE_CHILDREN_URL: baseAPIUrl + 'api/bnois/employee-childrens/',
        CURRENT_STATUS_URL: baseAPIUrl + 'api/bnois/current-status/',
        PROMOTION_POLICY_URL: baseAPIUrl + 'api/bnois/promotion-policies/',
        TRACE_SETTING_URL: baseAPIUrl + 'api/bnois/trace-settings/',
        COURSE_POINT_URL: baseAPIUrl + 'api/bnois/course-points/',
        POINT_DEDUCTION_PUNISHMENT_URL: baseAPIUrl + 'api/bnois/point-deduction-for-punishments/',
        BONUS_POINT_MEDAL_URL: baseAPIUrl + 'api/bnois/bonus-point-medals/',
        BONUS_POINT_AWARD_URL: baseAPIUrl + 'api/bnois/bonus-point-awards/',
        POOR_COURSE_RESULT: baseAPIUrl + 'api/bnois/poor-course-results/',
        BRANCH_COUNTRY_WISE_COURSE_POINT: baseAPIUrl + 'api/bnois/bra-ctry-course-points/',
        BONUS_POINT_PUBLICATION_URL: baseAPIUrl + 'api/bnois/bonus-point-publics/',
        BONUS_POINT_COMMENDATION_APPRECIATION_URL: baseAPIUrl + 'api/bnois/bonus-point-com-apps/',
        OPR_ENTRY_URL: baseAPIUrl + 'api/bnois/opr-entries/',
        OPR_APPOINTMENT_URL: baseAPIUrl + 'api/bnois/opr-special-appointments/',
        SEA_SERVICE_URL: baseAPIUrl + 'api/bnois/sea-services/',
        EMPLOYEE_COURSE_FUTURE_PLAN_URL: baseAPIUrl + 'api/bnois/employee-course-future-plans/',
        EMPLOYEE_TRANSFER_FUTURE_PLAN_URL: baseAPIUrl + 'api/bnois/employee-transfer-future-plans/',
        CERTIFICATE_URL: baseAPIUrl + 'api/bnois/certificates/',
        RETIRED_EMPLOYEE_URL: baseAPIUrl + 'api/bnois/retired-employees/',
        EMPLOYEE_RREPORT_URL: baseAPIUrl + 'api/bnois/employee-reports/',
        REPORT_URL: baseAPIUrl + 'api/bnois/reports/',
        TRANSFER_PROPOSAL_URL: baseAPIUrl + 'api/bnois/transfer-proposals/',
        PROPOSAL_DETAIL_URL: baseAPIUrl + 'api/bnois/proposal-details/',
        PROPOSAL_CANDIDATE_URL: baseAPIUrl + 'api/bnois/proposal-candidates/',
        ADVANCE_SEARCH_URL: baseAPIUrl + 'api/bnois/advance-search/',
        BASIC_SEARCH_URL: baseAPIUrl + 'api/bnois/basic-search/',
        RESULT_GRADE_URL: baseAPIUrl + 'api/bnois/result-grades/',
        DASHBOARD_URL: baseAPIUrl + 'api/bnois/dashboard/',
        SHIP_MOVEMENT_URL: baseAPIUrl + 'api/bnois/ship-movement/', 
        DAILY_PROCESS_URL: baseAPIUrl + 'api/bnois/daily-processes/',
        BACK_LOG_URL: baseAPIUrl + 'api/bnois/back-logs/',
        STATUS_CHANGE_URL: baseAPIUrl + 'api/bnois/status-changes/',
        EXTRA_APPOINTMENT_URL: baseAPIUrl + 'api/bnois/extra-appointments/',
        REMARK_URL: baseAPIUrl + 'api/bnois/remarks/',
        FOREIGN_PROJECT_URL: baseAPIUrl + 'api/bnois/foreign-projects/',
        PREVIOUS_LEAVE_URL: baseAPIUrl + 'api/bnois/previous-leaves/',
        PREVIOUS_TRANSFER_URL: baseAPIUrl + 'api/bnois/previous-transfers/',
        PREVIOUS_PUNISHMENT_URL: baseAPIUrl + 'api/bnois/previous-punishments/',
        PREVIOUS_MISSION_URL: baseAPIUrl + 'api/bnois/previous-missions/',
        QUICK_LINK_URL: baseAPIUrl + 'api/bnois/quick-links/',
        SPECIAL_APPOINTMENT_URL: baseAPIUrl + 'api/bnois/special-apt-types/',
        EMPLOYEE_HAJJ_DETAILS_URL: baseAPIUrl + 'api/bnois/employee-hajj-details/',
        EVIDENCE_URL: baseAPIUrl + 'api/bnois/evidences/',
        EMPLOYEE_CAR_LOAN_URL: baseAPIUrl + 'api/bnois/employee-car-loan/',
        CAR_LOAN_FISCAL_YEAR_URL: baseAPIUrl + 'api/bnois/car-loan-fiscal-year/',
        MSC_EDUCATION_TYPE_URL: baseAPIUrl + 'api/bnois/msc-education-type/',
        MSC_INSTITUTE_URL: baseAPIUrl + 'api/bnois/msc-institute/',
        MSC_PERMISSION_TYPE_URL: baseAPIUrl + 'api/bnois/msc-permission-type/',
        EMPLOYEE_MSC_EDUCATION_URL: baseAPIUrl + 'api/bnois/employee-msc-education/',
        CAREER_FORECAST_SETTING_URL: baseAPIUrl + 'api/bnois/career-forecast-setting/',
        CAREER_FORECAST_URL: baseAPIUrl + 'api/bnois/career-forecast/',
        BNOIS_LOG_INFO_URL: baseAPIUrl + 'api/bnois/bnois-log-info/',
        BNOIS_COXO_SERVICE_URL: baseAPIUrl + 'api/bnois/employee-coxo-service/',
        ROASTER_LIST_URL: baseAPIUrl + 'api/bnois/roaster-list/',
        TOE_AUTHORIZED_URL: baseAPIUrl + 'api/bnois/toe-authorized/',
        OODENTRY_URL: baseAPIUrl + 'api/bnois/overview-officers-deployment-entry/',
        TOEOSENTRY_URL: baseAPIUrl + 'api/bnois/toe-officer-state-entry/',
    });
})();