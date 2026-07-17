import {z} from 'zod';

const requiredString = (fieldName: string) => z.string({error: `${fieldName} is required`})
    .min(1, {error: `${fieldName} is required`});

export const activitySchema = z.object({
    title: requiredString('Title'),
    description: requiredString('Description'),
    category: requiredString('Category'),
    date: z.coerce.date<Date>({
        error: 'Date is required'
    }),
    location: z.object({
        venue: requiredString('venue'),
        city: z.string().optional(),
        latitude: z.coerce.number<number>(),
        longitude: z.coerce.number<number>(),
    })
});

export type ActivitySchema = z.infer<typeof activitySchema>;