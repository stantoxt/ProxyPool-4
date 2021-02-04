
CREATE TABLE IF NOT EXISTS `t_proxys` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ip` varchar(32) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT '',
  `port` int(11) NOT NULL DEFAULT '0',
  `anonymous_degree` int(11) NOT NULL DEFAULT '1',
  `score` int(11) NOT NULL DEFAULT '1',
  `created_time` datetime DEFAULT NULL,
  `updated_time` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `unique_port_ip` (`port`,`ip`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;