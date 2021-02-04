using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogFilterWeb.Utility
{
    public static class Constants
    {
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        public const string DateFormat = "yyyy-MM-dd";

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss,fff
        /// </summary>
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";


        /// <summary>
        /// X:\\Processed\\SUOS\\
        /// </summary>
        public const string SUOSRoot = "X:\\Processed\\SUOS\\";

        /// <summary>
        /// X:\\Logs\\SUOS\\
        /// </summary>
        public const string SUOSLogsRoot = "X:\\Logs\\SUOS\\";

        /// <summary>
        /// "6", "7"
        /// </summary>
        public static readonly string[] SUOSMachines = { "6", "7" };

        /// <summary>
        /// otp
        /// </summary>
        public const string SUOSOtpConfig = "otp";

        /// <summary>
        /// buster
        /// </summary>
        public const string SUOSBusterConfig = "buster";

        /// <summary>
        /// default
        /// </summary>
        public const string SUOSDefaultConfig = "default";

        public static readonly string[] SUOSConfigurations = { SUOSDefaultConfig, SUOSBusterConfig, SUOSOtpConfig };



        /// <summary>
        /// X:\\Processed\\SmartUCF\\
        /// </summary>
        public const string SmartUCFRoot = "X:\\Processed\\SmartUCF\\";

        /// <summary>
        /// X:\\Logs\\SmartUCF\\
        /// </summary>
        public const string SmartUCFLogsRoot = "X:\\Logs\\SmartUCF\\";

        /// <summary>
        /// "06", "07", "08", "09"
        /// </summary>
        public static readonly string[] SmartUCFOldMachines = { "01", "02", "04", "05" };
        public static readonly string[] SmartUCFNewMachines = { "06", "07", "08", "09" };

        public static string[] SmartUCFMachines => SmartUCFOldMachines.Concat(SmartUCFNewMachines).ToArray();

        /// <summary>
        /// default
        /// </summary>
        public const string SmartUCFDefaultConfig = "default";

        public static readonly string[] SmartUCFConfigurations = { SmartUCFDefaultConfig };

        public static readonly Dictionary<string, string> SmartUCFListDisplayName = new Dictionary<string, string>()
        {
            {"q_accp_credit_cycle", "Списък кредитни цикли"},
            {"Q_appl_decision_UW", "Списък 'Искания за одобрение'"},
            {"Q_appl_deliv_pr", "Списък 'Искания за ППП'"},
            {"q_client_econsent", "Получено Е-съгласие за клиент"},
            {"q_crd_post_calc_plan", "Заявки за постфактум разсрочване на транзакции"},
            {"Q_liq_notpaid", "Списък 'Ликвидирани неплатени заеми'"},
            {"q_list_agent", "Списък Агенти"},
            {"q_list_appl", "Списък искания за кредит (Общ)"},
            {"Q_list_appl_disb_C", "Списък 'Заеми за плащане на клиенти'"},
            {"Q_list_appl_for_disb", "Списък 'Заеми за плащане на търговци'"},
            {"q_list_appl_mine", "Списък искания, регистрирани от мен"},
            {"q_list_appl_pos_sel", "Списък искания за кредит (POS Seller)"},
            {"q_list_appl_restore", "Списък 'Искания за възстановяване'"},
            {"Q_LIST_APPL_REVIEWER", "Списък искания за кредит (Reviewer)"},
            {"q_list_appl_seller", "Списък искания за кредит (Seller)"},
            {"q_list_appl_trade", "Списък искания за кредит (Trade Representative)"},
            {"q_list_branch", "Бранчове"},
            {"q_list_card_product", "Списък 'Картови продукти'"},
            {"q_list_client", "Списък клиенти"},
            {"Q_list_client_appl", "Търсене на клиент по номер на искане"},
            {"q_list_client_phone", "Търсене на клиент по телефон"},
            {"q_list_country", "Държави"},
            {"Q_list_cpi_package", "Списък 'Застрахователни пакети'"},
            {"Q_list_cpi_type", "Списък 'Застраховки'"},
            {"q_list_crd_calc_plan", "Заявки от DMCARD за разсрочване на транзакции"},
            {"q_list_crd_type_tax", "Списък 'Типове карти и такси за разсрочване'"},
            {"q_list_cvm_actual", "Актуален CVM списък"},
            {"q_list_cvm_cont_data", "CVM последен контакт с клиент"},
            {"Q_list_deliv_pr", "Списък на Приемо-предавателни протоколи"},
            {"q_list_dmcrd_shp_map", "Съответствие на магазини с DMCARD"},
            {"q_list_email_client", "Списък шаблони за email за клиент"},
            {"q_list_email_otp", "Списък шаблони за email за ОТП"},
            {"q_list_emails", "Заявки за Email към онлайн заявки"},
            {"q_list_fc_upload", "Списък искания изпратени към FlexCube"},
            {"Q_List_FinTable_full", "Списък 'Финансови таблици'"},
            {"q_list_if70", "IF70 събития"},
            {"q_list_kimv", "Коефициенти за изчисление на месечна вноска"},
            {"q_list_kimv_otp", "Коефициенти за изчисление на месечна вноска"},
            {"Q_list_log", "Списък 'Записи в системния журнал'"},
            {"Q_list_log_details", "Списък 'Детайлна информация за запис в системния журнал'"},
            {"q_list_logo_activ", "Конфигуриране на картинка за заглавна страница"},
            {"q_list_mobile_agent", "Списък мобилни агенти"},
            {"Q_list_notifications", "Списък нотификации"},
            {"q_list_online_req", "Онлайн заявки"},
            {"q_list_onreq_in_que", "Онлайн заявки в опашки"},
            {"q_list_onreq_to_proc", "Недовършени онлайн заявки"},
            {"q_list_otp", "Онлайн търговски партньори"},
            {"q_list_phone_prefix", "Телефонни префикси"},
            {"q_list_post_code_nom", "Населени места и пощенски кодове"},
            {"q_list_postsales_req", "Списък заявки за следпродажбено обслужване"},
            {"q_list_preappr_cards", "Предварително одобрени КК по ЕГН"},
            {"q_list_print_pdf", "Зареждане на съдържание на бланки"},
            {"Q_list_printouts", "Списък 'Бланки на документи за печат'"},
            {"q_list_printouts_ext", "Бланки на документи за печат"},
            {"Q_list_product", "Списък 'Продукти' - стар, не се използва ??!!!"},
            {"Q_List_Product_full", "Списък 'Продукти'"},
            {"q_list_ps_req_queue", "Обработка на заявки за следпродажбено обслужване"},
            {"Q_list_qappl", "Списък 'Искания за ликвидация'"},
            {"q_list_queue", "Списък с опашки"},
            {"q_list_queue_appl_LI", "Искания в опашки за ликвидация"},
            {"q_list_queue_appl_UW", "Искания в опашки за одобрение"},
            {"q_list_queue_cont_ps", "Заявки в опашки за обслужване"},
            {"q_list_region", "Списък региони"},
            {"q_list_sales_network", "Списък 'Търговска верига'"},
            {"q_list_send_to_dmcrd", "Искания изпратени към DMCARD"},
            {"q_list_send_to_insrg", "Искания изпратени към РЗ"},
            {"q_list_sms", "Заявки за SMS"},
            {"q_list_sms_template", "Списък шаблони за SMS"},
            {"q_list_sn_distrib", "Разпространени финансови таблици"},
            {"q_list_sn_for_ft", "Избор на магазини за добавяне на разпространение"},
            {"Q_list_user_login", "Списък 'История на достъп на потребител'"},
            {"q_lst_onident_to_prc", "Онлайн заявки със забавена идентификация"},
            {"q_m_groups_list", "Списък роли"},
            {"q_m_roles_custom", "Права за достъп до бланки и препратки"},
            {"q_m_users_list", "Списък потребители"},
            {"Q_masspmt_files", "Списък 'Файлове за масово плащане'"},
            {"Q_masspmt_loans", "Списък 'Платени суми по търговец'"},
            {"Q_masspmt_loans_rpt", "Справка 'Платени суми на търговец за период'"},
            {"Q_masspmt_rows", "Списък 'Преводи от файл за масово плащане'"},
            {"q_menu_blanki", "Меню Бланки"},
            {"q_menu_blanki_Sub", "Поделементи на меню Бланки"},
            {"q_reg_card_clsd_unpd", "Закрити контракти за КК с непогасен дълг"},
            {"v_list_appl_POS", "Списък 'Искания за кредит'"},
        };

    }

    public enum SortOrder
    {
        Unknown,

        Asc,
        Desc,
    }
}
