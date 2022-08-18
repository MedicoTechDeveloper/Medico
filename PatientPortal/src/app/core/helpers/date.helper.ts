import * as moment from 'moment';

export class DateHelper {

    static jsLocalDateToSqlServerUtc(dateString: string): string {
        return moment.utc(dateString).toISOString();
    }

    static sqlServerUtcDateToLocalJsDate(sqlServerDateString: string): Date | null {
        if (!sqlServerDateString)
            return null;

        const utcServerDate = `${sqlServerDateString}Z`;
        return new Date(utcServerDate);
    }

    static getDaysBetween(startDate: Date, endDate: Date = null): number {
        const end = endDate ? endDate : new Date();

        if (!end.getTime || !startDate.getTime)
            return 0;

        const timeDiff = Math.abs(end.getTime() - startDate.getTime());
        return Math.ceil(timeDiff / (1000 * 3600 * 24));
    }

    static getAge(dateOfBirth: Date): number {
        const now = new Date();

        var years = (now.getFullYear() - dateOfBirth.getFullYear());

        if (now.getMonth() < dateOfBirth.getMonth() ||
            now.getMonth() == dateOfBirth.getMonth() && now.getDate() < dateOfBirth.getDate()) {
            years--;
        }

        return years;
    }

    static getFullDate(date: any): string {
        return moment(date).format("MMMM Do YYYY, h:mm");
    }

    static getTime(date: any): string {
        return moment(date).locale("en").format("HH:mm")
    }

    static getDate(date: Date): string {
        return moment(date).locale("en").format("DD/MM/YYYY");
    }

    static getUtcOffset(): number {
        return - (new Date()).getTimezoneOffset() / 60;
    }
}