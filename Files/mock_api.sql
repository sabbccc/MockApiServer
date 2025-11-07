
-- mock_api_server_db.applications definition

CREATE TABLE `applications` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` varchar(100) DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` varchar(100) DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- mock_api_server_db.mocks definition

CREATE TABLE `mocks` (
  `id` int NOT NULL AUTO_INCREMENT,
  `application_id` int NOT NULL,
  `name` varchar(255) DEFAULT NULL,
  `path` varchar(255) NOT NULL,
  `method` varchar(20) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` varchar(100) DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` varchar(100) DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `fk_mocks_applications` (`application_id`),
  CONSTRAINT `fk_mocks_applications` FOREIGN KEY (`application_id`) REFERENCES `applications` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;


-- mock_api_server_db.mock_scenarios definition

CREATE TABLE `mock_scenarios` (
  `id` int NOT NULL AUTO_INCREMENT,
  `mock_id` int NOT NULL,
  `scenario_key` varchar(100) NOT NULL,
  `status_code` int NOT NULL,
  `response_json` text NOT NULL,
  `headers_json` text,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` varchar(100) DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` varchar(100) DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`),
  KEY `fk_mock_scenarios_mocks` (`mock_id`),
  CONSTRAINT `fk_mock_scenarios_mocks` FOREIGN KEY (`mock_id`) REFERENCES `mocks` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=57 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `login` (
    `id` BIGINT NOT NULL AUTO_INCREMENT,
    `username` VARCHAR(150) NULL,
    `password` VARCHAR(255) NULL,
    `remember_me` TINYINT(1) NULL DEFAULT 0,
    `login_attempt_time` DATETIME NOT NULL,
    `ip_address` VARCHAR(50) NULL,
    `user_agent` VARCHAR(500) NULL,
    `device_info` VARCHAR(255) NULL,
    `location` VARCHAR(255) NULL,
    `is_successful` TINYINT(1) NULL DEFAULT 1,
    `session_id` CHAR(36) NOT NULL,
    `remarks` VARCHAR(500) NULL,
    PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO applications (id, name, created_by)
VALUES (1, 'DESCO', 'system');


INSERT INTO mocks (id, application_id, name, path, method, created_by)
VALUES
(1, 1, 'DESCO - Sign In', '/billpayment/global/auth/signin', 'POST', 'system'),
(2, 1, 'DESCO - Bill Info', '/billpayment/global/billvendor/billInfo', 'POST', 'system'),
(3, 1, 'DESCO - Payment Info', '/billpayment/global/billvendor/paymentInfo', 'POST', 'system');


INSERT INTO mock_scenarios (mock_id, scenario_key, status_code, response_json, created_by) VALUES
(1, 'success', 200, '{"username":"your_username","status":"ok","accessToken":"eyJhbGciOiJIUzUxMiJ9","tokenType":"Bearer"}', 'system'),
(1, 'invalid-username', 301, '{"error":"Username is incorrect!"}', 'system'),
(1, 'invalid-password', 302, '{"error":"Password is incorrect!"}', 'system'),
(1, 'inactive-user', 303, '{"error":"User Account Is Not Active."}', 'system');

INSERT INTO mock_scenarios (mock_id, scenario_key, status_code, response_json, created_by) VALUES
(2, 'success', 200,
 '{"status":"ok","billNo":"021737204495","billToken":"NapXuhfEYuCG","accountNo":"37204495","meterNo":"053773","year":"2017","month":"2","totalAmount":1640.0,"totalVat":75.0,"issueDate":"15/02/2017","departmentCode":"37","dueDate":"16/03/2017","lpc":"78","tariff":"A","consumerName":"MD.JOYNAL ABDEN","address":"House/Flat #704/501, BORODEWRA TONGI, Contact no: 01739246740","paymentType":"MONTHLY_BILL","paymentStatus":"PAID","totalAmountTobePaid":1715.0,"organizationCode":"2","totalKwh":"70"}',
 'system'),
(2, 'bill-missing', 304, '{"error":"Bill Number Is Required."}', 'system'),
(2, 'bill-invalid', 305, '{"error":"Bill Number Not Found."}', 'system'),
(2, 'server-error', 315, '{"error":"Please Try Again Later."}', 'system');


INSERT INTO mock_scenarios (mock_id, scenario_key, status_code, response_json, created_by) VALUES
(3, 'success', 200, '{"status":"ok","statusCode":"323","message":"Bill Payment Information Received."}', 'system'),
(3, 'bill-missing', 304, '{"error":"Bill Number Is Required."}', 'system'),
(3, 'bill-invalid', 305, '{"error":"Bill Number Not Found."}', 'system'),
(3, 'token-missing', 306, '{"error":"Bill Token Is Required."}', 'system'),
(3, 'token-invalid', 307, '{"error":"Bill Token Does Not Match."}', 'system'),
(3, 'transaction-missing', 308, '{"error":"Transaction Id Is Required."}', 'system'),
(3, 'bill-paid', 309, '{"error":"Bill Already Paid."}', 'system'),
(3, 'amount-mismatch', 310, '{"error":"Material/Security/Fees/Bill Amount Does Not Match."}', 'system'),
(3, 'bankcode-missing', 312, '{"error":"Bank Code Is Required."}', 'system'),
(3, 'scrollno-missing', 334, '{"error":"Scroll No Is Required."}', 'system'),
(3, 'paymentdate-missing', 335, '{"error":"Payment Date Is Required."}', 'system'),
(3, 'txndatetime-missing', 336, '{"error":"Transaction Date Time Is Required."}', 'system'),
(3, 'deptcode-missing', 337, '{"error":"Department Code Is Required."}', 'system'),
(3, 'paymentamount-missing', 338, '{"error":"Payment Amount Is Required."}', 'system'),
(3, 'totalpaid-missing', 340, '{"error":"Total Paid Amount Is Required."}', 'system'),
(3, 'totalpayable-missing', 341, '{"error":"Total Payable Amount Is Required."}', 'system'),
(3, 'invalid-fields', 327, '{"error":"Please Check Field’s Value."}', 'system'),
(3, 'server-error', 315, '{"error":"Please Try Again Later."}', 'system');
