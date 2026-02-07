-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: localhost    Database: mock_api_server_db
-- ------------------------------------------------------
-- Server version	8.0.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `applications`
--

DROP TABLE IF EXISTS `applications`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `applications` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` varchar(100) DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` varchar(100) DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `applications`
--

LOCK TABLES `applications` WRITE;
/*!40000 ALTER TABLE `applications` DISABLE KEYS */;
INSERT INTO `applications` VALUES (1,'DESCO','2025-09-27 20:06:30','system',NULL,NULL,1),(2,'DPDC','2025-10-01 12:31:19','system',NULL,NULL,1);
/*!40000 ALTER TABLE `applications` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mock_scenarios`
--

DROP TABLE IF EXISTS `mock_scenarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB AUTO_INCREMENT=80 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mock_scenarios`
--

LOCK TABLES `mock_scenarios` WRITE;
/*!40000 ALTER TABLE `mock_scenarios` DISABLE KEYS */;
INSERT INTO `mock_scenarios` VALUES (1,1,'success',200,'{\"username\":\"username\",\"status\":\"ok\",\"accessToken\":\"eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJia2FzaCIsImlhdCI6MTYyNDg1NjI1MSwiZXhwIjoxNjI0ODU5ODUxfQ.Jor9nIFrtXx9dErVc10dg_lY1qGmtCumuNxRIpwnkzVR3wNGtqs0astHACe3AUR_hDyZm8AFZsceHcuS5ehqA\",\"tokenType\":\"Bearer\"}',NULL,'2025-09-27 20:06:37','system','2025-09-27 20:07:55',NULL,1),(2,1,'invalid-username',301,'{\"status\":\"failed\",\"statusCode\":\"301\",\"message\":\"Username is incorrect!\"}',NULL,'2025-09-27 20:06:37','system','2025-09-27 20:07:55',NULL,1),(3,1,'invalid-password',302,'{\"status\":\"failed\",\"statusCode\":\"302\",\"message\":\"Password is incorrect!\"}',NULL,'2025-09-27 20:06:37','system','2025-09-27 20:07:55',NULL,1),(4,1,'inactive-user',303,'{\"status\":\"failed\",\"statusCode\":\"303\",\"message\":\"User Account Is Not Active.\"}',NULL,'2025-09-27 20:06:37','system','2025-09-27 20:07:55',NULL,1),(5,2,'success',200,'{\"status\":\"ok\",\"billNo\":\"021737204495\",\"billToken\":\"NapXuhfEYuCG\",\"accountNo\":\"37204495\",\"meterNo\":\"053773\",\"year\":\"2017\",\"month\":\"2\",\"totalAmount\":1640.0,\"totalVat\":75.0,\"issueDate\":\"15/02/2017\",\"departmentCode\":\"37\",\"dueDate\":\"16/03/2017\",\"lpc\":\"78\",\"tariff\":\"A\",\"consumerName\":\"MD.JOYNAL ABDEN\",\"address\":\"House/Flat #704/501, BORODEWRA TONGI, Contact no: 01739246740\",\"paymentType\":\"MONTHLY_BILL\",\"paymentStatus\":\"PAID\",\"totalAmountTobePaid\":1715.0,\"organizationCode\":\"2\",\"totalKwh\":\"70\"}',NULL,'2025-09-27 20:06:41','system','2025-09-27 20:07:55',NULL,1),(6,2,'bill-missing',304,'{\"status\":\"failed\",\"statusCode\":\"304\",\"message\":\"Bill Number Is Required.\"}',NULL,'2025-09-27 20:06:41','system','2025-09-27 20:07:55',NULL,1),(7,2,'bill-invalid',305,'{\"status\":\"failed\",\"statusCode\":\"305\",\"message\":\"Bill Number Not Found.\"}',NULL,'2025-09-27 20:06:41','system','2025-09-27 20:07:55',NULL,1),(8,2,'server-error',315,'{\"status\":\"failed\",\"statusCode\":\"315\",\"message\":\"Please Try Again Later.\"}',NULL,'2025-09-27 20:06:41','system','2025-09-27 20:07:55',NULL,1),(9,3,'success',200,'{\"status\":\"ok\",\"statusCode\":\"323\",\"message\":\"Bill Payment Information Received.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(10,3,'bill-missing',304,'{\"status\":\"failed\",\"statusCode\":\"304\",\"message\":\"Bill Number Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(11,3,'bill-invalid',305,'{\"status\":\"failed\",\"statusCode\":\"305\",\"message\":\"Bill Number Not Found.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(12,3,'token-missing',306,'{\"status\":\"failed\",\"statusCode\":\"306\",\"message\":\"Bill Token Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(13,3,'token-invalid',307,'{\"status\":\"failed\",\"statusCode\":\"307\",\"message\":\"Bill Token Does Not Match.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(14,3,'transaction-missing',308,'{\"status\":\"failed\",\"statusCode\":\"308\",\"message\":\"Transaction Id Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(15,3,'invalid-fields',327,'{\"status\":\"failed\",\"statusCode\":\"327\",\"message\":\"Please Check Field’s Value.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(16,3,'bankcode-missing',312,'{\"status\":\"failed\",\"statusCode\":\"312\",\"message\":\"Bank Code Time Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(17,3,'scrollno-missing',334,'{\"status\":\"failed\",\"statusCode\":\"334\",\"message\":\"Scroll No Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(18,3,'paymentdate-missing',335,'{\"status\":\"failed\",\"statusCode\":\"335\",\"message\":\"Payment Date Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(19,3,'txndatetime-missing',336,'{\"status\":\"failed\",\"statusCode\":\"336\",\"message\":\"Transaction Date Time Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(20,3,'deptcode-missing',337,'{\"status\":\"failed\",\"statusCode\":\"337\",\"message\":\"Department Code Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(21,3,'paymentamount-missing',338,'{\"status\":\"failed\",\"statusCode\":\"338\",\"message\":\"Payment Amount Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(22,3,'totalpaid-missing',340,'{\"status\":\"failed\",\"statusCode\":\"340\",\"message\":\"Total Paid Amount Is Required.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(24,3,'bill-paid',309,'{\"status\":\"failed\",\"statusCode\":\"309\",\"message\":\"Bill Already Paid.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(25,3,'amount-mismatch',310,'{\"status\":\"failed\",\"statusCode\":\"310\",\"message\":\"Material/Security/Fees/Bill Amount Does Not Match.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(26,3,'server-error',315,'{\"status\":\"failed\",\"statusCode\":\"315\",\"message\":\"Please Try Again Later.\"}',NULL,'2025-09-27 20:06:45','system','2025-09-27 20:07:55',NULL,1),(27,4,'success',200,'{\"status\":\"ok\",\"data\":[{\"address\":\"X11 D/1 18 MIRPUR DHAKA.\",\"billingMonth\":1,\"departmentCode\":\"13\",\"stampQty\":0,\"paymentAmount\":56.0,\"paymentVatAmount\":3.0,\"collectionDate\":\"16-06-2021 03:38:34 PM\",\"transactionId\":\"20111301039477121\",\"paymentChannel\":\"Dutch-Bangla Bank\",\"billingYear\":2020,\"billNumber\":\"012031291311\",\"paymentDate\":\"16-06-2021 04:23:53 PM\",\"paymentStatus\":\"Paid\",\"consumerName\":\"MR SHAHIDUL ISLAM\"}]}',NULL,'2025-09-27 20:06:49','system','2025-09-27 20:07:55',NULL,1),(28,4,'bill-missing',304,'{\"status\":\"failed\",\"statusCode\":\"304\",\"message\":\"Bill Number Is Required.\"}',NULL,'2025-09-27 20:06:49','system','2025-09-27 20:07:55',NULL,1),(29,4,'server-error',315,'{\"status\":\"failed\",\"statusCode\":\"315\",\"message\":\"Please Try Again Later.\"}',NULL,'2025-09-27 20:06:49','system','2025-09-27 20:07:55',NULL,1),(79,3,'totalpayable-missing',341,'{\"status\":\"failed\",\"statusCode\":\"341\",\"message\":\"Total Payable Amount Is Required.\"}',NULL,'2025-09-27 20:06:45','system',NULL,NULL,1);
/*!40000 ALTER TABLE `mock_scenarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mocks`
--

DROP TABLE IF EXISTS `mocks`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
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
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mocks`
--

LOCK TABLES `mocks` WRITE;
/*!40000 ALTER TABLE `mocks` DISABLE KEYS */;
INSERT INTO `mocks` VALUES (1,1,'DESCO - Sign In','/billpayment/global/auth/signin','POST','2025-09-27 20:06:34','system',NULL,NULL,1),(2,1,'DESCO - Bill Info','/billpayment/global/billvendor/billInfo','POST','2025-09-27 20:06:34','system',NULL,NULL,1),(3,1,'DESCO - Payment Info','/billpayment/global/billvendor/paymentInfo','POST','2025-09-27 20:06:34','system',NULL,NULL,1),(4,1,'DESCO - Payment Status','/billpayment/global/billvendor/paymentStatus','POST','2025-09-27 20:06:34','system',NULL,NULL,1);
/*!40000 ALTER TABLE `mocks` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `username` varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  `password` varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  `name` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `mobile_no` varchar(255) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `last_login_time` datetime DEFAULT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `updated_by` varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `is_active` tinyint(1) NOT NULL DEFAULT '1',
  `remarks` varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'sabbccc','aLek9gZMVKHL+8WhhyUA3OMhYki3NlrCFjpN0M2ybaE=','Sabbir Hossain','01770457939',NULL,'2025-10-21 23:43:16',NULL,'2025-10-21 23:46:40',NULL,1,NULL);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'mock_api_server_db'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-01-25  0:15:18
