import { DialogDefault, Snackbar } from '@3mo/model'

export class Service {
	static notify(...parameters: Parameters<typeof Snackbar.show>) {
		return Snackbar.show(...parameters)
	}

	static throwAndNotify(errorOrErrorMessage: Error | string) {
		this.notify(typeof errorOrErrorMessage === 'string' ? errorOrErrorMessage : errorOrErrorMessage.message)
		throw typeof errorOrErrorMessage === 'string' ? new Error(errorOrErrorMessage) : errorOrErrorMessage
	}

	static confirmDeletion(handleDeletion: () => Promise<void>, parameters?: { heading?: string, content?: string }) {
		return new DialogDefault({
			heading: parameters?.heading || 'Confirm Deletion',
			content: parameters?.content || 'Are you sure you want to delete this irreversibly?',
			primaryButtonText: 'Delete',
			primaryButtonAction: () => handleDeletion(),
			secondaryButtonText: 'Cancel',
		}).confirm()
	}
}